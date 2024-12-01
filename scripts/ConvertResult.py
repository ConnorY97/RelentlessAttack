import defusedxml.ElementTree as ET
import json
import argparse
import os
from datetime import datetime

def to_unix_timestamp(dt_str):
    """
    Converts an ISO 8601 datetime string to a Unix timestamp in seconds.
    Returns 0 if conversion fails.
    """
    try:
        dt = datetime.strptime(dt_str, "%Y-%m-%d %H:%M:%S%z")
        return int(dt.timestamp())
    except Exception as e:
        print(f"Error parsing datetime '{dt_str}': {e}")
        return 0

def convert_xml_to_json(xml_file_path, output_file_path):
    try:
        # Parse the XML file
        tree = ET.parse(xml_file_path)
        root = tree.getroot()

        # Locate the main test-suite for start and end times
        main_suite = root.find("./test-suite")
        if main_suite is not None:
            start_time = main_suite.attrib.get("start-time", "0001-01-01 00:00:00Z")
            end_time = main_suite.attrib.get("end-time", "0001-01-01 00:00:00Z")
        else:
            start_time = "0001-01-01 00:00:00Z"
            end_time = "0001-01-01 00:00:00Z"

        start_timestamp = to_unix_timestamp(start_time)
        stop_timestamp = to_unix_timestamp(end_time)

        print(f"Start time {start_timestamp}")
        print(f"Stop time {stop_timestamp}")

        # General Information
        total_tests = int(root.attrib.get('total', 0))
        passed_tests = int(root.attrib.get('passed', 0))
        failed_tests = int(root.attrib.get('failed', 0))
        skipped_tests = int(root.attrib.get('skipped', 0))

        # Prepare test details
        total_duration = 0
        tests = []
        for testcase in root.findall(".//test-case"):
            name = testcase.attrib.get("name", "Unnamed Test")
            status = testcase.attrib.get("result", "unknown").lower()
            duration = float(testcase.attrib.get("duration", 0)) * 1000  # Convert to ms
            total_duration += duration
            suite = testcase.attrib.get("classname", "Unknown Suite")
            test_start = to_unix_timestamp(testcase.attrib.get("start-time", start_time)) * 1000
            test_stop = to_unix_timestamp(testcase.attrib.get("end-time", end_time)) * 1000
            message = ""
            trace = ""

            # Handle failure details
            failure_element = testcase.find(".//failure")
            if failure_element is not None:
                message = failure_element.findtext("message", "No message provided")
                trace = failure_element.findtext("stack-trace", "No trace available")

            # Create the test entry
            test_entry = {
                "name": name,
                "status": status,
                "duration": int(duration),
                "start": test_start,
                "stop": test_stop,
                "suite": suite,
                "rawStatus": status,
                "tags": [],
                "type": "e2e",  # Example type, modify if necessary
                "filePath": "",  # File path not provided in XML
                "retries": 0,
                "flaky": False,
                "browser": "Unknown",  # Modify if browser info is available
                "extra": {},
                "message": message,
                "trace": trace,
                "screenshot": None  # Screenshots not available in XML
            }
            tests.append(test_entry)

        stop_timestamp = start_time + int(total_duration)
        print(f"Final stop after adding duration {stop_timestamp}")
        # Summary Section
        summary = {
            "tests": total_tests,
            "passed": passed_tests,
            "failed": failed_tests,
            "pending": 0,  # No pending info in XML
            "skipped": skipped_tests,
            "other": 0,  # No "other" info in XML
            "suites": len(root.findall(".//test-suite")),  # Count the test suites
            "start": start_timestamp,
            "stop": stop_timestamp,
            "duration": int(total_duration)
        }

        # Final Output
        final_output = {
            "results": {
                "tool": {
                    "name": "RelentlessAttack",  # Modify if tool name changes
                    "version": "1.0"  # Modify if version changes
                },
                "summary": summary,
                "tests": tests
            }
        }

        # Write JSON file
        os.makedirs(os.path.dirname(output_file_path), exist_ok=True)
        with open(output_file_path, "w") as json_file:
            json.dump(final_output, json_file, indent=4)

        print(f"Conversion successful! JSON saved to {output_file_path}")

    except Exception as e:
        print(f"Error during conversion: {e}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Convert XML test results to JSON.")
    parser.add_argument("xml_file", help="Path to the input XML file.")
    args = parser.parse_args()

        # Ensure the output directories exist
    json_output_file  = "/home/connor/Documents/Results/result.json"

    os.makedirs(os.path.dirname(json_output_file), exist_ok=True)
    print("Created file")
    convert_xml_to_json(args.xml_file, json_output_file)
