using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NotepadV2.Native
{
    public static class NativeInterop
    {
        [DllImport("FileDialogNative.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int LoadTextFile(StringBuilder buffer, int bufSize);

        [DllImport("FileDialogNative.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int SaveTextFile(string text);

        public static string LoadTextViaDialog()
        {
            StringBuilder buffer = new StringBuilder(65536);
            int result = LoadTextFile(buffer, buffer.Capacity);
            return result == 1 ? buffer.ToString() : null;
        }

        public static bool SaveTextViaDialog(string content)
        {
            return SaveTextFile(content) == 1;
        }
    }
}
