import defusedxml.ElementTree as ET
import json
import argparse
import os
from datetime import datetime

def convert_xml_to_json(xml_file_path, output_file_path):
    try:
        # Parse the XML file
        tree = ET.parse(xml_file_path)
        root = tree.getroot()

        # General Information
        total_tests = int(root.attrib.get('total', 0))
        passed_tests = int(root.attrib.get('passed', 0))
        failed_tests = int(root.attrib.get('failed', 0))
        skipped_tests = int(root.attrib.get('skipped', 0))
        start_time = root.attrib.get('start-time', "0001-01-01 00:00:00Z")
        end_time = root.attrib.get('end-time', "0001-01-01 00:00:00Z")

        def to_unix_timestamp(dt_str):
            """Converts ISO 8601 datetime string to Unix timestamp in seconds."""
            try:
                dt = datetime.fromisoformat(dt_str.replace('Z', '+00:00'))
                return int(dt.timestamp())
            except ValueError:
                return 0  # Default to 0 if conversion fails

        start_timestamp = to_unix_timestamp(start_time)
        stop_timestamp = to_unix_timestamp(end_time)

        # Prepare test details
        total_duration = 0  # To sum up test durations
        tests = []
        for testcase in root.findall(".//test-case"):
            name = testcase.attrib.get("name", "Unnamed Test")
            status = testcase.attrib.get("result", "unknown").lower()
            duration = float(testcase.attrib.get("duration", 0)) * 1000  # Convert to ms
            print(f"Test duration {int(duration)}")
            total_duration += int(duration)
            print(f"New total duration {int(total_duration)}")
            suite = testcase.attrib.get("classname", "Unknown Suite")
            start = to_unix_timestamp(testcase.attrib.get("start-time", start_time)) * 1000
            stop = to_unix_timestamp(testcase.attrib.get("end-time", end_time)) * 1000
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
                "start": start,
                "stop": stop,
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
        print(f"Finaly total duration {total_duration}")
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
            "duration": 100000000#int(total_duration)
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
