#pragma once
#include "AssetsFileFormat.h"

#include "defines.h"

class AssetFileInfoEx : public AssetFileInfo
{
	public:
		//AssetsHeader format < 0x10 : equals curFileTypeOrIndex
		//AssetsHeader format >= 0x10 : equals TypeTree.pTypes_Unity5[curFileTypeOrIndex].classId or (DWORD)-2 if the index is out of bounds
		DWORD curFileType;
		QWORD absolutePos;
		char name[100];
};

//File tables make searching assets easier
//The names are always read from the asset itself if possible,
//	see AssetBundleFileTable or ResourceManagerFile for more/better names
class AssetsFileTable
{
	AssetsFile *pFile;
	AssetsFileReader reader;
	LPARAM readerPar;

	public:
		AssetFileInfoEx *pAssetFileInfo;
		unsigned int assetFileInfoCount;

	public:
		//Reading names requires a high random access, set readNames to false if you don't need them
		ASSETSTOOLS_API AssetsFileTable(AssetsFile *pFile, bool readNames=true);
		ASSETSTOOLS_API ~AssetsFileTable();

		ASSETSTOOLS_API AssetFileInfoEx *getAssetInfo(const char *name);
		ASSETSTOOLS_API AssetFileInfoEx *getAssetInfo(const char *name, DWORD type);
		ASSETSTOOLS_API AssetFileInfoEx *getAssetInfo(QWORD pathId);
		
		ASSETSTOOLS_API AssetsFile *getAssetsFile();
		ASSETSTOOLS_API AssetsFileReader getReader();
		ASSETSTOOLS_API LPARAM getReaderPar();
};
