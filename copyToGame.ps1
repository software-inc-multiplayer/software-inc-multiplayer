Try {
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Core.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\DLLMods\Multiplayer"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Core.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\DLLMods\Multiplayer"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Facepunch.Steamworks.Win64.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Facepunch.Steamworks.Win64.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Facepunch.Steamworks.Win64.xml" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Google.Protobuf.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Debugging.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Debugging.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Extensions.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Extensions.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Networking.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Networking.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Shared.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\Multiplayer.Shared.pdb" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\System.Buffers.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\System.Memory.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\System.Numerics.Vectors.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Copy-Item -Force -Path ".\Multiplayer.Core\bin\Debug\net472\System.Runtime.CompilerServices.Unsafe.dll" -Destination "D:\Program Files (x86)\Steam\steamapps\common\Software Inc\Software Inc_Data\Managed"
    Write-Host "Done"
}
Catch {
    echo "Couldn't deploy:"
    echo $_
}


