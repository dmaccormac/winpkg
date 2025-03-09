
#include "resource.h"
#include <fstream>
#include <iostream>
#include <ostream>
#include <string>
#include <windows.h> // Include Windows.h for resource functions

std::string wrapInQuotes(const std::string& str) {
    return "\"" + str + "\"";
}

bool extractResource(int resourceId, const std::string& outputPath) {
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

std::string getTempPath() {
    wchar_t tempPath[MAX_PATH];
    GetTempPathW(MAX_PATH, tempPath);
    std::wstring tempPathWStr(tempPath);
    return std::string(tempPathWStr.begin(), tempPathWStr.end());
}

bool createArchive(std::string source, std::string destination) {
    std::string tempPath = getTempPath();
    std::string command = tempPath + "7zr a -bso0 " + wrapInQuotes(destination) + " " + wrapInQuotes(source);
    system(command.c_str());
    return true;
}

bool createConfigFile(const std::string& app, const std::string& filePath) {
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
    std::cout << "winpkg 1.3.3\n";

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
    std::string tempPath = getTempPath();

    //extract 7zr.exe
    std::string tempFile = tempPath + "7zr.exe";
    if (!extractResource(IDR_7ZR, tempFile)) {
        std::cerr << "Failed to extract 7zr.exe" << std::endl;
        return 1;
    }

    //extract 7zSD.sfx
    tempFile = tempPath + "7zSD.sfx";
    if (!extractResource(IDR_7ZSD, tempFile)) {
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
        if (!extractResource(IDR_SETUP, tempFile)) {
            std::cerr << "Failed to extract setup.bat" << std::endl;
            return 1;
        }
    }

    //create config file
    tempFile = tempPath + "winpkg.conf";
    if (!createConfigFile(app, tempFile)) {
        std::cerr << "Failed to create config file" << std::endl;
        return 1;
    }

    std::cout << "Building package...\n";

    //create 7z archive
	tempFile = tempPath + app + ".7z";
	createArchive(source, tempFile);

    // create sfx archive
    std::string sfx = tempPath + "7zSD.sfx+";
    std::string conf = tempPath + "winpkg.conf+";
    std::string archive = wrapInQuotes(tempFile) + " ";
    std::string command = "copy /b " + sfx + conf + archive + wrapInQuotes(app + ".exe") + " > NUL";
    system(command.c_str());

    // clean up
    command = "del /q " + wrapInQuotes(tempFile);
    system(command.c_str());

    std::cout << "Done." << std::endl;

}