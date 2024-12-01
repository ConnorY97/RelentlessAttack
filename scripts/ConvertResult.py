import defusedxml.ElementTree as ET
import json
import argparse
import os

def convert_xml_to_json(xml_file_path, output_file_path):
    try:
        # Parse the XML file
        tree = ET.parse(xml_file_path)
        root = tree.getroot()

        # Extract general information
        total_tests = int(root.attrib.get('total', 0))
        passed_tests = int(root.attrib.get('passed', 0))
        failed_tests = int(root.attrib.get('failed', 0))
        skipped_tests = int(root.attrib.get('skipped', 0))

        # Initialize total duration accumulator
        total_duration = 0

        # Extract test case details
        tests = []
        for testcase in root.findall(".//test-case"):
            name = testcase.attrib.get('name', 'Unknown')
            result = testcase.attrib.get('result', 'unknown').lower()
            duration = float(testcase.attrib.get('duration', 0)) * 1000  # Convert to ms
            total_duration += duration  # Add to total duration

            # Failure message handling
            failure_element = testcase.find(".//failure")
            failure_message = (
                failure_element.find("message").text
                if failure_element is not None and failure_element.find("message") is not None
                else None
            )

            # Create test entry
            test_entry = {
                "name": name,
                "status": result,
                "duration": duration,
                "message": failure_message,
                "suite": testcase.attrib.get('classname', 'Unknown Suite'),
                "flaky": False,  # Modify based on additional context if necessary
            }
            tests.append(test_entry)

        # Prepare summary
        summary = {
            "tests": total_tests,
            "passed": passed_tests,
            "failed": failed_tests,
            "skipped": skipped_tests,
            "other": 0,  # Placeholder for untracked statuses
            "duration": total_duration,  # Total duration in ms
        }

        # Final JSON structure
        final_output = {
            "results": {
                "tool": {
                    "name": "RelentlessAttack",  # Tool name, modify as needed
                    "version": "1.0",  # Version, modify as needed
                },
                "summary": summary,
                "tests": tests,
            }
        }

        # Write to JSON file
        os.makedirs(os.path.dirname(output_file_path), exist_ok=True)
        with open(output_file_path, 'w') as json_file:
            json.dump(final_output, json_file, indent=4)

        print(f"Conversion successful! JSON saved to {output_file_path}")

    except Exception as e:
        print(f"Error during conversion: {e}")

if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Convert XML test results to JSON.")
    parser.add_argument("xml_file", help="Path to the input XML file.")
    parser.add_argument("output_file", help="Path to the output JSON file.")
    args = parser.parse_args()

    convert_xml_to_json(args.xml_file, args.output_file)

