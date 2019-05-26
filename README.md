Random code to try and get custom songs on the Quest

Get BeatmapAssetMaker, for this example put it in .\BeatmapAssetMaker
Grab songe-converter from https://github.com/lolPants/songe-converter/releases
Grab a custom song from beatsaver, e.g. https://beatsaver.com/download/7798-7744
Unzip the custom song somewhere, e.g. .\Paqqin
run songe-converter to convert the song from old format to the new format, e.g. songe-converter Paqqin
run BeatmapAssetMaker to create the beatmap asset
	I *think* the name of the asset matters for the name of the track/difficulty you are replacing, but maybe not, e.g. EscapeExpertPlusBeatmapData
	The path to the dat file is to the specific difficulty map in the new format
	The output filename is where you want the asset written
	e.g. .\BeatmapAssetMaker EscapeExpertPlusBeatmapData .\Paqqin\ExpertPlus.dat .\PaqqinExpertPlus.assetdata
Use the tool of your choice to unpack the quest beatsaver unity assets.  I found most of the songs in shared asset bundle 17.
Find the MonoBehavior asset of the track you're replacing (e.g. EscapeExpertPlusBeatmapData)
Replace the raw data of that asset with the assetdata file  you created