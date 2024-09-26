import defusedxml.ElementTree as ET
import json
import time
import argparse
import os
from PIL import Image, ImageDraw, ImageFont
from datetime import datetime, timezone
import traceback
import re

# def xml_to_json(xml_file, tool_name):
#     # Check if the file exists
#     if not os.path.exists(xml_file):
#         raise FileNotFoundError(f"The file {xml_file} does not exist.")
#     else:
#         print("XML File exists")

#     # Parse the XML file securely
#     tree = ET.parse(xml_file)
#     root = tree.getroot()

#     # Initialize summary counts
#     total_tests = 0
#     passed_tests = 0
#     failed_tests = 0
#     pending_tests = 0
#     skipped_tests = 0
#     other_tests = 0

#     # Extract test cases
#     test_cases = []
#     for testcase in root.findall('testcase'):
#         total_tests += 1
#         status = testcase.get('status', 'unknown')
#         if status == 'run':
#             passed_tests += 1
#             # Rename to correctly post
#             status = 'passed'
#         elif status == 'fail':
#             failed_tests += 1
#             status = 'failed'
#         elif status == 'skipped':
#             skipped_tests += 1
#         else:
#             other_tests += 1

#         # Getting a message if there is one
#         system_out = testcase.find('system-out')
#         message = system_out.text if system_out is not None else ''

#         test_case = {
#             "name": testcase.get('name'),
#             "status": status,
#             "duration": int(float(testcase.get('time', 0)) * 1000),  # Convert to milliseconds, default to 0 if not present
#             "message": message
#         }
#         test_cases.append(test_case)

#     # Get the 'timestamp' attribute of the 'testsuite' element
#     timestamp_str = root.get('timestamp')
#     if timestamp_str:
#         # Convert the timestamp string to a datetime object
#         timestamp_dt = datetime.strptime(timestamp_str, '%Y-%m-%dT%H:%M:%S')

#         # Convert the datetime object to seconds since the epoch
#         timestamp_seconds = int(timestamp_dt.replace(tzinfo=timezone.utc).timestamp())
#     else:
#         # Default timestamp if not present
#         timestamp_seconds = int(datetime.now(timezone.utc).timestamp())

#     # Get the current time in seconds since the epoch
#     current_time_seconds = int(datetime.now(timezone.utc).timestamp())

#     # Construct the summary
#     summary = {
#         "tests": total_tests,
#         "passed": passed_tests,
#         "failed": failed_tests,
#         "pending": pending_tests,
#         "skipped": skipped_tests,
#         "other": other_tests,
#         "start": timestamp_seconds,
#         "stop": current_time_seconds
#     }

#     # Construct the JSON structure
#     json_structure = {
#         "results": {
#             "tool": {
#                 "name": tool_name
#             },
#             "summary": summary,
#             "tests": test_cases
#         }
#     }
#     print(json_structure)
#     return json_structure

def xml_to_json(xml_file, tool_name):
    # Check if the file exists
    if not os.path.exists(xml_file):
        raise FileNotFoundError(f"The file {xml_file} does not exist.")
    else:
        print("XML File exists")

    # Parse the XML file securely
    tree = ET.parse(xml_file)
    root = tree.getroot()

    # Initialize summary counts
    total_tests = 0
    passed_tests = 0
    failed_tests = 0
    pending_tests = 0
    skipped_tests = 0
    other_tests = 0

    # Extract test cases
    test_cases = []
    for suite in root.findall(".//test-case"):
        total_tests += 1
        status = suite.get('result', 'unknown')
        if status == 'Passed':
            passed_tests += 1
            status = 'passed'
        elif status == 'Failed':
            failed_tests += 1
            status = 'failed'
        elif status == 'Skipped':
            skipped_tests += 1
        else:
            other_tests += 1

        # Get message if available
        message = ''
        system_out = suite.find('system-out')
        if system_out is not None:
            message = system_out.text or ''

        test_case = {
            "name": suite.get('name'),
            "status": status,
            "duration": int(float(suite.get('duration', 0)) * 1000),  # Convert to milliseconds
            "message": message
        }
        test_cases.append(test_case)

    # Get the 'start-time' attribute from the 'test-run' element
    start_time_str = root.get('start-time')
    if start_time_str:
        timestamp_dt = datetime.strptime(start_time_str, '%Y-%m-%d %H:%M:%SZ')
        timestamp_seconds = int(timestamp_dt.replace(tzinfo=timezone.utc).timestamp())
    else:
        timestamp_seconds = int(datetime.now(timezone.utc).timestamp())

    # Get the current time in seconds since the epoch
    current_time_seconds = int(datetime.now(timezone.utc).timestamp())

    # Construct the summary
    summary = {
        "tests": total_tests,
        "passed": passed_tests,
        "failed": failed_tests,
        "pending": pending_tests,
        "skipped": skipped_tests,
        "other": other_tests,
        "start": timestamp_seconds,
        "stop": current_time_seconds
    }

    # Construct the JSON structure
    json_structure = {
        "results": {
            "tool": {
                "name": tool_name
            },
            "summary": summary,
            "tests": test_cases
        }
    }
    return json_structure

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Convert XML result to JSON.')
    parser.add_argument('xml_file', help="Path to the XML file")

    args = parser.parse_args()

    try:
        # Generate JSON data
        json_data = xml_to_json(args.xml_file, "Self hosted Runner")

        json_output_file = "/home/connor/Documents/Results/result.json"
        print(json_output_file)

        # Ensure the output directories exist
        os.makedirs(os.path.dirname(json_output_file), exist_ok=True)
        print("Created file")

        # Write JSON output to file
        with open(json_output_file, 'w') as json_file:
            json.dump(json_data, json_file, indent=2)
        print(f"JSON output has been written to {json_output_file}")
    except FileNotFoundError as e:
        print(f"File not found with error: {e}")
        exit(1)
    except Exception as e:
        tb = traceback.extract_tb(e.__traceback__)  # Extract traceback details
        for frame in tb:
            filename = frame.filename
            lineno = frame.lineno
            function_name = frame.name
            line = frame.line
            print(f"File: {filename}, Line: {lineno}, Function: {function_name}")
            print(f"Code: {line}")
        print(f"An error occurred: {e}")
        exit(1)