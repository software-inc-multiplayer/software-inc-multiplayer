Try {
    New-Item -Path . -Name "installer-binaries" -ItemType "directory"
    New-Item -Path "./installer-binaries/" -Name "mf" -ItemType "directory" -Force
    New-Item -Path "./installer-binaries/" -Name "manage" -ItemType "directory" -Force
    # Copy assets etc. 
    Copy-Item -Path ".\swinc.multiplayer\Assets\" -Destination ".\installer-binaries\mf\Assets\" -Recurse -Force
    Copy-Item -Path ".\swinc.multiplayer\Localization\" -Destination ".\installer-binaries\mf\" -Recurse -Force
    # .\swinc.multiplayer\Multiplayer.Core\bin\Debug\Multiplayer.Core.dll
    Copy-Item -Path ".\swinc.multiplayer\Multiplayer.Core\bin\Debug\Multiplayer.Core.dll" -Destination ".\installer-binaries\mf\" -Recurse
    Copy-Item -Path ".\swinc.multiplayer\Multiplayer.Networking\bin\Debug\Multiplayer.Networking.dll" -Destination ".\installer-binaries\manage\" -Recurse -Force
    Copy-Item -Path ".\swinc.multiplayer\Multiplayer.Extensions\bin\Debug\Multiplayer.Extensions.dll" -Destination ".\installer-binaries\manage\" -Recurse -Force
    Copy-Item -Path ".\swinc.multiplayer\Multiplayer.Debugging\bin\Debug\Multiplayer.Debugging.dll" -Destination ".\installer-binaries\manage\" -Recurse -Force
    # .\swinc.multiplayer\References
    Copy-Item -Path ".\swinc.multiplayer\References\Telepathy.dll" -Destination ".\installer-binaries\manage\" -Force
    Copy-Item -Path ".\swinc.multiplayer\References\Newtonsoft.Json.dll" -Destination ".\installer-binaries\manage\" -Force
    Copy-Item -Path ".\swinc.multiplayer\References\zdiscord_game_sdk.dll" -Destination ".\installer-binaries\mf\" -Force
    Compress-Archive -Path ".\installer-binaries\*" -DestinationPath ".\installer-binaries.zip" -Force
} 
Catch {
    echo "Couldn't make installer-binaries:"
    echo $_
}