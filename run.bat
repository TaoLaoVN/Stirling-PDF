@echo off
setlocal
cd /d "%~dp0"

set "JAVA_EXE=%USERPROFILE%\.gradle\jdks\eclipse_adoptium-25-amd64-windows.2\bin\javaw.exe"
if not exist "%JAVA_EXE%" (
    set "JAVA_EXE=javaw"
)

:: Kiem tra backend da chay chua qua port 8080
powershell -NoProfile -Command "try { $r = (Invoke-WebRequest -Uri 'http://localhost:8080/api/v1/info/status' -UseBasicParsing -TimeoutSec 1).StatusCode; exit 0 } catch { exit 1 }"
if %ERRORLEVEL% NEQ 0 (
    echo Dang khoi dong MaPDF backend chay ngam...
    start "" "%JAVA_EXE%" -jar "%~dp0app\core\build\libs\stirling-pdf-3.1.0.jar" --server.port=8080
    
    :: Cho backend san sang (toi da 40s)
    for /L %%i in (1,1,40) do (
        timeout /t 1 /nobreak >nul
        powershell -NoProfile -Command "try { $r = (Invoke-WebRequest -Uri 'http://localhost:8080/api/v1/info/status' -UseBasicParsing -TimeoutSec 1).StatusCode; exit 0 } catch { exit 1 }"
        if !ERRORLEVEL! EQU 0 goto :ready
    )
)

:ready
:: Mo cua so ung dung Native (khong co thanh URL, khong co tab trinh duyet)
set "EDGE_PATH=C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"
if not exist "%EDGE_PATH%" (
    set "EDGE_PATH=C:\Program Files\Microsoft\Edge\Application\msedge.exe"
)

start "" "%EDGE_PATH%" --app=http://localhost:8080/ --user-data-dir="%LOCALAPPDATA%\MaPDF_Data" --window-size=1300,850
exit
