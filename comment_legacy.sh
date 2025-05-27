#!/bin/bash

ROOT_DIR="Assets/_Project/Scripts/Legacy"

find "$ROOT_DIR" -type f -name "*.cs" | while read -r file; do
  echo "Processing $file"
  
  # Check if file already starts with /* and ends with */
  first_line=$(head -n 1 "$file" | tr -d '[:space:]')
  last_line=$(tail -n 1 "$file" | tr -d '[:space:]')
  
  if [[ "$first_line" == "/*" && "$last_line" == "*/" ]]; then
    echo "Already commented out, skipping."
    continue
  fi
  
  # Wrap file contents with /* ... */
  # Save original content temporarily
  tmpfile=$(mktemp)
  echo "/*" > "$tmpfile"
  cat "$file" >> "$tmpfile"
  echo "*/" >> "$tmpfile"
  
  # Replace original file with the commented version
  mv "$tmpfile" "$file"
done

echo "All .cs files commented out."
