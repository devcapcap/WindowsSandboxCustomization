@echo off

set variables="C:\Program Files (x86)\dotnet;C:\Program Files\dotnet;C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit;C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn;C:\Program Files\Microsoft SQL Server\130\Tools\Binn"

for %%p in (%variables%) do (
    set "new_path=%%p"
    set "current_path=%PATH%"
    echo %current_path% | findstr /i /c:"%new_path%" >nul
    if %errorlevel%==0 (
        echo The %new_path% already in the PATH.
    ) else (
        ::setx PATH "%current_path%;%new_path%" /M
        echo The %new_path% added to PATH.
        ::set "PATH=%PATH%;%new_path%"
    )
)
