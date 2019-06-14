== Quest Beat Saber Stuff == 

Core code for reading and writing the APKs and asset files that make up Beat Saber's resources.  Also a winforms viewer for apk/asset files and bundle files and folders.

Also starting on an android port so it can all be done on the device.

BeatmapAssetMaker is a command line utility using the core library which can:
-Import a folder of custom songs (in the new Beat Saber native level editor format) to a playlist on the main menu
-Export a JSON file with the current configuration of songs and playlists (includes base64 cover art so that the image could be displayed in a consuming app)
-Import a JSON file with an udpated configuration of songs and playlists to add/remove songs, add/remove playlists and move songs around in playlists
	-Optional custom image for the playlist cover
	
-Apply binary patches (not really needed at the moment since the Beat Games crew very nicely removed the signature code)



== Credits ==

Credits and thanks to lots of people on the BSMG Discord, but naming a few specifically:

-Sc2ad (https://github.com/sc2ad) has helped a ton through code contribution, discovery, conversation and other ways.

-trishume (https://github.com/trishume) has implemented a comparable library and it's always helpful to compare aproaches.

-elliotttate was the launching point to make most of this possible and continuously offers ideas and suggestions what to do, where to look and how to approach things.

-NyanBlade has provided lots of help testing and writing/distributing a package to make my utility actually useable by people

-jakibaki (https://github.com/jakibaki/) for his (soon to be included) hooking library
