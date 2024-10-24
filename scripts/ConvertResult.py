
import defusedxml.ElementTree as ET
import json
import argparse
import os
from datetime import datetime, timezone
import traceback

def convert_xml_to_json(xml_file_path, output_file_path):
    try:
        # Parse the XML file
        tree = ET.parse(xml_file_path)
        root = tree.getroot()

        # Extract the general information
        total_tests = int(root.attrib.get('total', 0))
        passed_tests = int(root.attrib.get('passed', 0))
        failed_tests = int(root.attrib.get('failed', 0))
        start_time = root.attrib.get('start-time', None)
        end_time = root.attrib.get('end-time', None)

        # Debug output for start and end time
        print(f"Start time: {start_time}")
        print(f"End time: {end_time}")

        # Convert times to timestamps (assuming ISO 8601 format)
        def safe_fromisoformat(dt_str):
            if dt_str == '0001-01-01 00:00:00Z':
                print("Detected invalid timestamp, returning default value.")
                return 0  # or you could use int(datetime.now().timestamp())
            try:
                return int(datetime.fromisoformat(dt_str.replace('Z', '')).timestamp())
            except ValueError as ve:
                print(f"ValueError: {ve} for input: {dt_str}")
                return 0  # Return 0 or some default value if conversion fails
            except TypeError as te:
                print(f"TypeError: {te} for input: {dt_str}")
                return 0  # Return 0 or some default value if conversion fails

        start_timestamp = safe_fromisoformat(start_time) if start_time else 0
        end_timestamp = safe_fromisoformat(end_time) if end_time else 0


        # Prepare summary section
        summary = {
            "tests": total_tests,
            "passed": passed_tests,
            "failed": failed_tests,
            "pending": 0,
            "skipped": 0,  # No skipped tests info provided
            "other": 0,
            "suites": len(root.findall(".//test-suite")),  # Count the number of test suites
            "start": start_timestamp,
            "stop": end_timestamp
        }

        # Prepare tests section
        tests = []
        for testcase in root.findall(".//test-case"):
            name = testcase.attrib.get('name')
            result = testcase.attrib.get('result', 'unknown').lower()
            duration = float(testcase.attrib.get('duration', 0)) * 1000  # convert to ms
            test_start_time = testcase.attrib.get('start-time')
            test_end_time = testcase.attrib.get('end-time')

            test_start_timestamp = int(datetime.fromisoformat(test_start_time.replace('Z', '')).timestamp() * 1000) if test_start_time else 0
            test_end_timestamp = int(datetime.fromisoformat(test_end_time.replace('Z', '')).timestamp() * 1000) if test_end_time else 0

            # Handle errors and failure messages
            failure_message = ""
            failure = testcase.find(".//failure")
            if failure is not None:
                failure_message = failure.find("message").text if failure.find("message") is not None else "Unknown error"

            # Add test case details
            test_entry = {
                "name": name,
                "status": result,
                "duration": duration,
                "start": test_start_timestamp,
                "stop": test_end_timestamp,
                "suite": testcase.attrib.get('classname', 'Unknown Suite'),
                "rawStatus": result,
                "tags": [],
                "type": "e2e",  # This is a guess; modify based on your context if necessary
                "filePath": "",  # File path isn't available in the XML, so leaving empty
                "retries": 0,
                "flaky": False,
                "browser": "Unknown",  # Not provided in the XML
                "extra": {},
                "message": failure_message if failure_message else None
            }
            tests.append(test_entry)

        # Final JSON structure
        final_output = {
            "results": {
                "tool": {
                    "name": "RelentlessAttack",  # Modify based on your context
                    "version": "0.1"  # Modify based on your context
                },
                "summary": summary,
                "tests": tests
            }
        }

        # Write the JSON to the specified output file
        with open(output_file_path, 'w') as json_file:
            json.dump(final_output, json_file, indent=4)

        print(f"Conversion successful! JSON saved to {output_file_path}")
    except Exception as e:
        print(f"Error occurred during conversion: {e}")

def xml_to_json(xml_file, tool_name):
    # Check if the file exists
    if not os.path.exists(xml_file):
        raise FileNotFoundError(f"The file {xml_file} does not exist.")

    # Parse the XML file securely
    tree = ET.parse(xml_file)
    root = tree.getroot()

    # Initialize summary counts
    total_tests = 0
    passed_tests = 0
    failed_tests = 0
    skipped_tests = 0

    # Extract test cases
    test_cases = []
    for testcase in root.findall('testcase'):
        total_tests += 1
        status = testcase.get('status', 'unknown')

        # Determine the status of the test
        if status == 'run':
            passed_tests += 1
            status = 'passed'
        elif status == 'fail':
            failed_tests += 1
            status = 'failed'
        elif status == 'skipped':
            skipped_tests += 1

        # Extract message and stack trace, with defaults
        message = testcase.findtext('system-out', 'No details provided')
        trace = testcase.findtext('system-err', 'No trace available')

        test_case = {
            "name": testcase.get('name'),
            "status": status,
            "duration": int(float(testcase.get('time', 0)) * 1000),  # Convert to milliseconds, default to 0 if not present
            "message": message,
            "trace": trace
        }
        test_cases.append(test_case)

    # Get the timestamp attribute of the testsuite element
    timestamp_str = root.get('timestamp')
    if timestamp_str:
        # Convert the timestamp string to a datetime object
        timestamp_dt = datetime.strptime(timestamp_str, "%Y-%m-%dT%H:%M:%S")
        timestamp_dt = timestamp_dt.replace(tzinfo=timezone.utc).timestamp()
    else:
        # Use current timestamp if not available
        timestamp_dt = datetime.now().replace(tzinfo=timezone.utc).timestamp()

    # Build final JSON structure
    result_data = {
        "tool": "MyTestTool",
        "total": total_tests,
        "passed": passed_tests,
        "failed": failed_tests,
        "skipped": skipped_tests,
        "timestamp": int(timestamp_dt),
        "tests": test_cases
    }

    # Write the result data to the JSON output
    with open(output_json, 'w') as json_file:
        json.dump(result_data, json_file, indent=4)

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Convert XML test results to JSON.")
    parser.add_argument("xml_file", help="The input XML file containing test results.")
    args = parser.parse_args()

    output_json = "/home/connor/Documents/Results/result.json"
    try:
        # Generate JSON data
        #json_data = xml_to_json(args.xml_file, "Self hosted Runner")

        json_output_file = "/home/connor/Documents/Results/result.json"
        print(json_output_file)

        # Ensure the output directories exist
        os.makedirs(os.path.dirname(json_output_file), exist_ok=True)
        print("Created file")

        # # Write JSON output to file
        # with open(json_output_file, 'w') as json_file:
        #     json.dump(json_data, json_file, indent=2)
        # print(f"JSON output has been written to {json_output_file}")

        convert_xml_to_json(args.xml_file, json_output_file)

    except FileNotFoundError as e:
        print(f"File not found with error: {e}")
        exit(1)
    except Exception as e:
        traceback.print_exc()
        print(f"Failed to convert XML to JSON: {e}")
