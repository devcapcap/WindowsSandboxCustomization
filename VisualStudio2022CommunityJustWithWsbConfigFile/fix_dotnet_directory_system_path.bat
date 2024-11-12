@echo off

set variables="C:\Program Files (x86)\dotnet;C:\Program Files\dotnet;C:\Program Files (x86)\Windows Kits\10\Windows Performance Toolkit;C:\Program Files\Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn;C:\Program Files\Microsoft SQL Server\130\Tools\Binn"

for %%p in (%variables%) do (
    set "new_path=%%p"
    set "current_path=%PATH%"
    :: Vérifier si le chemin est déjà dans PATH pour éviter les doublons
    echo %current_path% | findstr /i /c:"%new_path%" >nul
    if %errorlevel%==0 (
        echo Le chemin %new_path% est déjà dans PATH.
    ) else (
        :: Ajouter le nouveau chemin à PATH
        ::setx PATH "%current_path%;%new_path%" /M
        echo Le chemin %new_path% a été ajouté à PATH.
        
        :: Mettre à jour PATH dans la session actuelle
        ::set "PATH=%PATH%;%new_path%"
    )
)