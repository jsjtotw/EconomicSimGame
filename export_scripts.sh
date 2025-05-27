#!/bin/bash
cd /Users/student/Documents/EconomicSimGame-alpha/ || exit
OUTPUT_FILE="all_scripts.txt"
> "$OUTPUT_FILE"
find Assets/_Project/Scripts/ -type f -name "*.cs" ! -path "*/Legacy/*" | while read -r file; do
    echo "// File: $file" >> "$OUTPUT_FILE"
    cat "$file" >> "$OUTPUT_FILE"
    echo -e "\n" >> "$OUTPUT_FILE" 
done
echo "Export complete: $OUTPUT_FILE"