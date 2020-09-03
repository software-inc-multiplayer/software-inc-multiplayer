Try {
    New-Item -Path . -Name "installer-binaries" -ItemType "directory"
    New-Item -Path "./installer-binaries/" -Name "mf" -ItemType "directory"
    New-Item -Path "./installer-binaries/" -Name "manage" -ItemType "directory"
    # Copy assets etc. 
    Copy-Item -Path ".\swinc.multiplayer\Assets\*" -Destination ".\installer-binaries\mf\Assets\" -Recurse
    Copy-Item -Path ".\swinc.multiplayer\Localization\*" -Destination ".\installer-binaries\mf\Localization\" -Recurse
} 
Catch {
    echo "Couldn't make installer-binaries:"
    echo $_
}