#include <windows.h>
#include <stdio.h>

__declspec(dllexport) int LoadTextFile(char* buffer, int bufSize) {
    OPENFILENAMEA ofn = { 0 };
    char path[MAX_PATH] = "";
    ofn.lStructSize = sizeof(ofn);
    ofn.lpstrFilter = "Text Files\0*.txt\0All Files\0*.*\0";
    ofn.lpstrFile = path;
    ofn.nMaxFile = MAX_PATH;
    ofn.Flags = OFN_FILEMUSTEXIST;
    ofn.lpstrTitle = "Load Text File";

    if (GetOpenFileNameA(&ofn)) {
        FILE* fp = fopen(path, "r");
        if (!fp) return 0;
        fread(buffer, 1, bufSize - 1, fp);
        buffer[bufSize - 1] = '\0';
        fclose(fp);
        return 1;
    }
    return 0;
}

__declspec(dllexport) int SaveTextFile(const char* text) {
    OPENFILENAMEA ofn = { 0 };
    char path[MAX_PATH] = "";
    ofn.lStructSize = sizeof(ofn);
    ofn.lpstrFilter = "Text Files\0*.txt\0All Files\0*.*\0";
    ofn.lpstrFile = path;
    ofn.nMaxFile = MAX_PATH;
    ofn.Flags = OFN_OVERWRITEPROMPT;
    ofn.lpstrTitle = "Save Text File";

    if (GetSaveFileNameA(&ofn)) {
        FILE* fp = fopen(path, "w");
        if (!fp) return 0;
        fprintf(fp, "%s", text);
        fclose(fp);
        return 1;
    }
    return 0;
}