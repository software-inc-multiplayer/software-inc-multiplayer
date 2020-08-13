$compress = @{
  Path = "C:\Users\calum\source\repos\software-inc-multiplayer\Installer\bin\x64\Release"
  CompressionLevel = "Fastest"
  DestinationPath = "C:\coding\windows-x64.zip"
}
$compresse = @{
  Path = "C:\Users\calum\source\repos\software-inc-multiplayer\Installer\bin\x86\Release"
  CompressionLevel = "Fastest"
  DestinationPath = "C:\coding\windows-x86.zip"
}
Compress-Archive @compress
Compress-Archive @compresse