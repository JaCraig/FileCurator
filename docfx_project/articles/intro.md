# Code
[!code-csharp[](../../FileCurator.Example/Program.cs)]

# Output

```
File Info:
File: C:\*********\MyFile.txt
File Exists: True
File Extension: .txt
File Name: MyFile.txt
File length: 24

PDF Content:
This is a test docx

CSV Content:
Header 1 Header 2 Header 3 Header 4 Header 5 Header 6
This is a test CSV file
Tons of data in here is super
important
Columns: Header 1,Header 2,Header 3,Header 4,Header 5,Header 6
Rows: This, is, a, test, CSV, file
Tons, of, data, in here, is, super
important

"Header 1","Header 2","Header 3","Header 4","Header 5","Header 6"
"This","is","a","test","CSV","file"
"Tons","of","data","in here","is","super"
"important"
```