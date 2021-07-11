@ECHO OFF
TITLE "Set SoftwareInc Folder"ù
ECHO "REMEMBER TO RUN ME AS ADMINISTRATOR"
ECHO To speed up development and testing and team-working, we'll create an environment variable
ECHO that links to the Software Inc installation folder.
ECHO This will be used to fix references to the game DLLs and automatically copy build artifacts to the game's mod folder.
ECHO You just need to specify where Software Inc is located. (Right click on the game on steam - Manage - Browse local files - copy the path here)
ECHO Usually it's something like xxx\steamapps\common\Software Inc
ECHO A new environment variable called %%softwareincfolder%% will be created.
ECHO To remove it use: "REG delete HKCU\Environment /F /V softwareincfolder" (it will take change at next logon)
ECHO Paste here ty:
set /p sincfolder="Software inc folder: "
setx softwareincfolder "%sincfolder%" /m
PAUSE