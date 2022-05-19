<div align="center">
  <a href="https://cal3432.github.io/software-inc-multiplayer"><img  src="https://raw.githubusercontent.com/cal3432/software-inc-multiplayer/master/swinc.multiplayer/Assets/Logo/Square44x44Logo.targetsize-256.png" /></a><br>
  <a href="https://github.com/cal3432/software-inc-multiplayer/issues"><img  src="https://img.shields.io/github/issues/cal3432/software-inc-multiplayer?style=for-the-badge" /></a>
  <a href="https://github.com/cal3432/software-inc-multiplayer/stargazers"><img  src="https://img.shields.io/github/stars/cal3432/software-inc-multiplayer?style=for-the-badge" /></a>
  <a href="https://github.com/cal3432/software-inc-multiplayer/releases"><img  src="https://img.shields.io/github/downloads/cal3432/software-inc-multiplayer/total?style=for-the-badge" /></a>
  <a href="https://sincmultiplayer.net/discord"><img width="156" height="28" src="https://i.imgur.com/RokeTxs.png" /></a>
  <hr>
  <h3>Software Inc Multiplayer Mod</h3>
  <p>A multiplayer mod for the game "Software Inc"</p>
</div>

<h2>Contents</h2>

- <a href="#installation">Installation</a>
  - [Manual Installation (Windows only, mainly for contributors)](#manual-installation)
- <a href="#how-it-works">How it works</a>
- <a href="#bug-reports-and-feature-requests">Submitting a bug report/feature request.</a>
- <a href="#contributing">Contributing</a>

## Installation

<!-- 

<a href="https://github.com/cal3432/software-inc-multiplayer/releases/tag/2.0.0-launcher"> Download the latest manager here.</a>

The manager is a desktop app you can use to install different versions of the mod, at a fast speed with no hassle. If you are having trouble using the installer, keep reading.

-->

###### Manual Installation (Windows/Contributors/Developers only)

1. Head to the _"Utilities"_ folder and run __as administrator__ the batch file called _"setSoftwareIncFolder.bat"_;
2. It will ask you to copy and paste the game's directory installation path (usually something like _"A:\SteamLibrary\steamapps\common\Software Inc"_, to find it quickly, right click on the game on Steam->Manage->Browse Local Files);
3. The batch file will then execute the following batch command to set an Environment variable called _softwareincfolder_:
	```batch
	setx softwareincfolder "%pathyoutcopypasted" /m
	```
4. All the references in the various projects inside the solution use the environment variable to point at the game root folder in order to avoid having to overwrite those references path at every push from a different person.
5. There are also post-build events that automatically move the required DLL to the proper game's folders so everything you need to do is just build and play the game to see the mod!

To remove the environment variable use the following batch command in an administrator prompt window:
```batch
REG delete HKCU\Environment /F /V softwareincfolder
```
And then a full computer restart for the changes to take effect.

## How it works

The multiplayer mod is based on the Peer To Peer system currently, with dedicated servers coming soon.

First, the client connects to the server and sends their "GameWorld", which is their company, their stocks, etc.
Secondly, the client will recieve a collection of GameWorlds from the other clients and merges them with theirs, showing other players companies and their stocks etc.
Lastly, any changes will be sent to the server to be sent to the other clients and vice versa.

## Bug Reports and Feature Requests

Anyone can submit a bug report <a href="https://github.com/cal3432/software-inc-multiplayer/issues">via the issues tab on GitHub</a>, please provide as much infomation as possible.
Same for feature requests.

## Contributing

See <a href="https://github.com/cal3432/software-inc-multiplayer/blob/nightly/CONTRIBUTING.md">CONTRIBUTING.md</a>




