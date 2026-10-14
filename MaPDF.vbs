Set WshShell = CreateObject("WScript.Shell")
WshShell.CurrentDirectory = "D:\Onedrive\Project-AI\Win - MaPDF"
WshShell.Run "cmd /c ""D:\Onedrive\Project-AI\Win - MaPDF\run.bat""", 0, False
Set WshShell = Nothing
