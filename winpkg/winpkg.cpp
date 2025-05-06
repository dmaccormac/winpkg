#include "resource.h"
#include <fstream>
#include <iostream>
#include <ostream>
#include <string>
#include <iomanip>
#include <sstream>
#include <windows.h> // Include Windows.h for resource functions
#include <vector>

/* 
* Function: concatenate_files
* Description: Concatenates multiple files into a single output file.
* Parameters:
* 	- inputFiles: A vector of strings containing the paths to the input files.
* 	- outputFile: The path to the output file where the concatenated content will be written.
* Returns: True if successful, false otherwise.
* Example:
* 	- concatenate_files({"file1.txt", "file2.txt"}, "output.txt");
*/

bool concatenate_files(const std::vector<std::string>& inputFiles, const std::string& outputFile) {
    std::ofstream out(outputFile, std::ios::binary); // Open output file in binary mode
    if (!out.is_open()) {
        std::cerr << "Failed to open output file: " << outputFile << std::endl;
        return false;
    }

    for (const auto& file : inputFiles) {
        std::ifstream in(file, std::ios::binary); // Open input file in binary mode
        if (!in.is_open()) {
            std::cerr << "Failed to open input file: " << file << std::endl;
            return false;
        }

        out << in.rdbuf(); // Append the contents of the input file to the output file
        in.close();
    }

    out.close();
    return true;
}

/*
* Function: quote_string
* Description: Wraps a string in double quotes. Useful for paths with spaces.
* Parameters:
* 	- str: The string to be quoted.
* Returns: The quoted string.
* Example:
* 	- quote_string("C:\Program Files\MyApp\app.exe");
*/

std::string quote_string(const std::string& str) {
    return "\"" + str + "\"";
    //std::ostringstream oss;
    //oss << std::quoted(str);
    //return oss.str();
}

//bool copy_file(const std::string& source, const std::string& destination) {
//    std::wstring wSource(source.begin(), source.end());
//    std::wstring wDestination(destination.begin(), destination.end());
//    return CopyFile(wSource.c_str(), wDestination.c_str(), FALSE);
//}

/*
* Function: extract_resource
* Description: Extracts a resource from the executable and saves it to a file.
* Parameters:
*	- resourceId: The ID of the resource to extract.
*	- outputPath: The path where the extracted resource will be saved.
* Returns: True if successful, false otherwise.
* Example:
*	- extract_resource(IDR_7ZR, "C:\temp\7zr.exe");
*/
bool extract_resource(int resourceId, const std::string& outputPath) {
    HRSRC hResource = FindResource(NULL, MAKEINTRESOURCE(resourceId), RT_RCDATA);
    if (!hResource) return false;

    HGLOBAL hLoadedResource = LoadResource(NULL, hResource);
    if (!hLoadedResource) return false;

    LPVOID pLockedResource = LockResource(hLoadedResource);
    DWORD dwResourceSize = SizeofResource(NULL, hResource);
    if (!pLockedResource || dwResourceSize == 0) return false;

    std::ofstream outputFile(outputPath, std::ios::binary);
    outputFile.write(reinterpret_cast<const char*>(pLockedResource), dwResourceSize);
    outputFile.close();

    return true;
}


/*
* Function: get_temp_path
* Description: Retrieves the temporary path for the current user.
* Parameters: None
* Returns: A string containing the temporary path.
* Example:
*	- std::string tempPath = get_temp_path();
*/
std::string get_temp_path() {
    wchar_t tempPath[MAX_PATH];
    GetTempPathW(MAX_PATH, tempPath);
    std::wstring tempPathWStr(tempPath);
    return std::string(tempPathWStr.begin(), tempPathWStr.end());
}


/*
* /Function: create_archive
* Description: Creates a 7z archive from the specified source directory.
* Parameters:
* - source: The path to the source directory to be archived.
* - destination: The path where the archive will be saved.
* Returns: True if successful, false otherwise.
* Example:
*       - create_archive("C:\\MyApp", "C:\\MyApp.7z");
*/
bool create_archive(std::string source, std::string destination) {
    std::string tempPath = get_temp_path();
    std::string command = tempPath + "7zr a -bso0 " + quote_string(destination) + " " + quote_string(source);
    system(command.c_str());
    return true;
}

/*
* Function: write_config_file
* Description: Writes a configuration file for the installer.
* Parameters:
*   - app: The name of the application.
*   - filePath: The path where the configuration file will be saved.
* Returns: True if successful, false otherwise.
* Example:
* 	  - write_config_file("MyApp", "C:\\MyApp\\winpkg.conf");
*/
bool write_config_file(const std::string& app, const std::string& filePath) {
    std::ofstream configFile(filePath);
    if (!configFile.is_open()) return false;

    configFile << ";!@Install@!UTF-8!" << std::endl;
    configFile << "Title=\"" + app +"\"" << std::endl;
    configFile << "BeginPrompt=\"Do you want to install " + app + "?\"" << std::endl;
    configFile << "RunProgram=\"" + app + "\\setup.bat\"" << std::endl;
    configFile << ";!@InstallEnd@!" << std::endl;

    configFile.close();
    return true;
}




int main(int argc, char* argv[])
{
    std::cout << "winpkg 1.3.4\n"; // Update setup.bat !

    if (argc < 2) {
        std::cout << "Package builder for Windows\n";
        std::cout << "Usage: winpkg <source>\n";

        return 1;
    }

    //get source folder
    std::string source = argv[1];

    //get app name
    if (source.back() == '\\') { source.pop_back(); }
    size_t pos = source.find_last_of("\\");
    std::string app = source.substr(pos + 1);

    // get temp path
    std::string tempPath = get_temp_path();

    //extract 7zr.exe
    std::string tempFile = tempPath + "7zr.exe";
    if (!extract_resource(IDR_7ZR, tempFile)) {
        std::cerr << "Failed to extract 7zr.exe" << std::endl;
        return 1;
    }

    //extract 7zSD.sfx
    tempFile = tempPath + "7zSD.sfx";
    if (!extract_resource(IDR_7ZSD, tempFile)) {
        std::cerr << "Failed to extract 7zSD.sfx" << std::endl;
        return 1;
    }

    //extract setup.bat
    tempFile = app + "\\setup.bat";

	// check if file exists
	if (std::ifstream(tempFile)) {
		std::cout << "Using existing setup.bat" << std::endl;
	}
    else
    {
        if (!extract_resource(IDR_SETUP, tempFile)) {
            std::cerr << "Failed to extract setup.bat" << std::endl;
            return 1;
        }
    }

    //create config file
    tempFile = tempPath + "winpkg.conf";
    if (!write_config_file(app, tempFile)) {
        std::cerr << "Failed to create config file" << std::endl;
        return 1;
    }

    std::cout << "Building package...\n";

    //create 7z archive
	tempFile = tempPath + app + ".7z";
	create_archive(source, tempFile);

    // Paths to files
    std::string sfx = tempPath + "7zSD.sfx";
    std::string conf = tempPath + "winpkg.conf";
    std::string archive = tempPath + app + ".7z";
    std::string outputExe = app + ".exe";


    // Combine the files into a single self-extracting archive
    std::vector<std::string> filesToCombine = { sfx, conf, archive };
    if (!concatenate_files(filesToCombine, outputExe)) {
        std::cerr << "Failed to create self-extracting archive" << std::endl;
        return 1;
    }

    // clean up
    std::string command = "del /q " + quote_string(tempFile);
    system(command.c_str());

    std::cout << "Done." << std::endl;

}