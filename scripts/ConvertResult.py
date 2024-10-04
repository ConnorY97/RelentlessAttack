
import defusedxml.ElementTree as ET
import json
import argparse
import os
from datetime import datetime, timezone
import traceback

def xml_to_json(xml_file, output_json):
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
        xml_to_json(args.xml_file, output_json)
        print(f"Conversion completed. JSON saved to {args.output_json}.")
    except Exception as e:
        traceback.print_exc()
        print(f"Failed to convert XML to JSON: {e}")
