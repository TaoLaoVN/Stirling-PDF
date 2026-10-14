@echo off
title Stirling-PDF (MaPDF) - Tieng Viet
echo ===================================================
echo     Dang khoi dong Stirling-PDF (MaPDF)...
echo     Giao dien mac dinh: Tieng Viet
echo     Mo trinh duyet tai: http://localhost:8080/
echo ===================================================

set "JAVA_CMD=%USERPROFILE%\.gradle\jdks\eclipse_adoptium-25-amd64-windows.2\bin\java.exe"
if not exist "%JAVA_CMD%" (
    set "JAVA_CMD=java"
)

"%JAVA_CMD%" -jar "%~dp0app\core\build\libs\stirling-pdf-3.1.0.jar" --server.port=8080
pause
