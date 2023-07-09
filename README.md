# Sln2Txt

This tool analyzes the content of all C# files in a given Visual Studio Solution (.sln), excluding those in "bin" and "obj" directories. It outputs the path and content of each file to an output text file.
It can be used to submit your Solution as single file to GPT 4 (code interpreter)

## Usage

1. Clone or download this repository to your local machine.

2. Open a command line or terminal and navigate to the directory of the downloaded code.

3. Run the program with the `dotnet run` command followed by the path to your .sln file and the path to the output file you wish to write the results to. 

    ```bash
    dotnet run "./path/to/your.sln" "./path/to/output.txt"
    ```

Please note that any errors encountered while reading a file will be displayed in the terminal.

## License

This project is licensed under the MIT License 
