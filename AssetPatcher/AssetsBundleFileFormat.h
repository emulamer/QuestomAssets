#pragma once
#ifndef __AssetsTools__AssetsBundleFormat_Header
#define __AssetsTools__AssetsBundleFormat_Header
#include "defines.h"
#include "BundleReplacer.h"
#include "ClassDatabaseFile.h"

class AssetsBundleFile;
struct AssetsBundleHeader06;
struct AssetsBundleHeader03;
struct AssetsBundleEntry;
struct AssetsBundleList;

struct AssetsBundleDirectoryInfo06
{
	QWORD offset;
	QWORD decompressedSize;
	DWORD flags;
	char *name;
	ASSETSTOOLS_API QWORD GetAbsolutePos(AssetsBundleHeader06 *pHeader);
	ASSETSTOOLS_API QWORD GetAbsolutePos(class AssetsBundleFile *pFile);
};
struct AssetsBundleBlockInfo06
{
	DWORD decompressedSize;
	DWORD compressedSize;
	WORD flags; //(flags & 0x40) : is streamed; (flags & 0x3F) :  compression info;
	inline BYTE GetCompressionType() { return (BYTE)(flags & 0x3F); }
	//example flags (LZMA, streamed) : 0x41
	//example flags (LZ4, not streamed) : 0x03
};
struct AssetsBundleBlockAndDirectoryList06
{
	QWORD checksumLow;
	QWORD checksumHigh;
	DWORD blockCount;
	AssetsBundleBlockInfo06 *blockInf;
	DWORD directoryCount;
	AssetsBundleDirectoryInfo06 *dirInf;
	
	ASSETSTOOLS_API void Free();
	ASSETSTOOLS_API bool Read(QWORD filePos, AssetsFileReader reader, LPARAM lPar, AssetsFileVerifyLogger errorLogger = NULL);
	//Write doesn't compress
	ASSETSTOOLS_API bool Write(AssetsFileWriter writer, LPARAM lPar, QWORD &curFilePos, AssetsFileVerifyLogger errorLogger = NULL);
};

#define LargestBundleHeader AssetsBundleHeader03
//Unity 5.3+
struct AssetsBundleHeader06
{
	//no alignment in this struct!
	char signature[13]; //0-terminated; UnityFS, UnityRaw, UnityWeb or UnityArchive
	DWORD fileVersion; //big-endian, = 6
	char minPlayerVersion[20]; //0-terminated; 5.x.x
	char fileEngineVersion[20]; //0-terminated; exact unity engine version
	QWORD totalFileSize;
	//sizes for the blocks info :
	DWORD compressedSize;
	DWORD decompressedSize;
	//(flags & 0x3F) is the compression mode (0 = none; 1 = LZMA; 2-3 = LZ4)
	//(flags & 0x40) says whether the bundle has directory info
	//(flags & 0x80) says whether the block and directory list is at the end
	DWORD flags;
	
	ASSETSTOOLS_API bool ReadInitial(AssetsFileReader reader, LPARAM lPar, AssetsFileVerifyLogger errorLogger = NULL);
	ASSETSTOOLS_API bool Read(AssetsFileReader reader, LPARAM lPar, AssetsFileVerifyLogger errorLogger = NULL);
	ASSETSTOOLS_API bool Write(AssetsFileWriter writer, LPARAM lPar, QWORD &curFilePos, AssetsFileVerifyLogger errorLogger = NULL);
	inline QWORD GetBundleInfoOffset()
	{
		if (this->flags & 0x80)
		{
			if (this->totalFileSize == 0)
				return -1;
			return this->totalFileSize - this->compressedSize;
		}
		else
		{
			//if (!strcmp(this->signature, "UnityWeb") || !strcmp(this->signature, "UnityRaw"))
			//	return 9;
			QWORD ret = strlen(minPlayerVersion) + strlen(fileEngineVersion) + 0x1A;
			if (this->flags & 0x100)
				return (ret + 0x0A);
			else
				return (ret + strlen(signature) + 1);
		}
	}
	inline DWORD GetFileDataOffset()
	{
		DWORD ret = 0;
		if (!strcmp(this->signature, "UnityArchive"))
			return this->compressedSize;
		else if (!strcmp(this->signature, "UnityFS") || !strcmp(this->signature, "UnityWeb"))
		{
			ret = (DWORD)strlen(minPlayerVersion) + (DWORD)strlen(fileEngineVersion) + 0x1A;
			if (this->flags & 0x100)
				ret += 0x0A;
			else
				ret += (DWORD)strlen(signature) + 1;
		}
		if (!(this->flags & 0x80))
			ret += this->compressedSize;
		return ret;
	}
};

struct AssetsBundleHeader03
{
	char signature[13]; //0-terminated; UnityWeb or UnityRaw
	DWORD fileVersion; //big-endian; 3 : Unity 3.5 and 4;
	char minPlayerVersion[20]; //0-terminated; 3.x.x -> Unity 3.x.x/4.x.x; 5.x.x
	char fileEngineVersion[20]; //0-terminated; exact unity engine version
	DWORD minimumStreamedBytes; //big-endian; not always the file's size
	DWORD bundleDataOffs; //big-endian;
	DWORD numberOfAssetsToDownload; //big-endian;
	DWORD levelCount; //big-endian;
	struct AssetsBundleOffsetPair *pLevelList;
	DWORD fileSize2; //big-endian; for fileVersion >= 2
	DWORD unknown2; //big-endian; for fileVersion >= 3
	BYTE unknown3;

	ASSETSTOOLS_API bool Read(AssetsFileReader reader, LPARAM lPar, AssetsFileVerifyLogger errorLogger = NULL);
	ASSETSTOOLS_API bool Write(AssetsFileWriter writer, LPARAM lPar, QWORD &curFilePos, AssetsFileVerifyLogger errorLogger = NULL);
	DWORD bundleCount; //big-endian;
};

struct AssetsBundleEntry
{
	DWORD offset;
	DWORD length;
	char name[1];
	ASSETSTOOLS_API unsigned int GetAbsolutePos(AssetsBundleHeader03 *pHeader);//, DWORD listIndex);
	ASSETSTOOLS_API unsigned int GetAbsolutePos(class AssetsBundleFile *pFile);//, DWORD listIndex);
};
struct AssetsList
{
	DWORD pos;
	DWORD count;
	AssetsBundleEntry **ppEntries;
	DWORD allocatedCount;
	//AssetsBundleEntry entries[0];
	ASSETSTOOLS_API void Free();
	ASSETSTOOLS_API bool Read(AssetsFileReader reader, LPARAM readerPar, QWORD &curFilePos, AssetsFileVerifyLogger errorLogger = NULL);
	ASSETSTOOLS_API bool Write(AssetsFileWriter writer, LPARAM writerPar, QWORD &curFilePos, AssetsFileVerifyLogger errorLogger = NULL);
	ASSETSTOOLS_API bool Write(AssetsFileReader reader, LPARAM readerPar, 
		AssetsFileWriter writer, LPARAM lPar, bool doWriteAssets, QWORD &curReadPos, QWORD *curWritePos = NULL,
		AssetsFileVerifyLogger errorLogger = NULL);
};
struct AssetsBundleOffsetPair
{
	DWORD compressed;
	DWORD uncompressed;
};

struct AssetsBundleFilePar
{
	AssetsBundleFile *pFile;
	union {
		AssetsBundleEntry *pEntry3;
		AssetsBundleDirectoryInfo06 *pEntry6;
	};
	DWORD listIndex;
	AssetsFileReader origFileReader;
	LPARAM origPar;
	QWORD curFilePos;
};
ASSETSTOOLS_API QWORD AssetsBundle_AssetsFileReader(QWORD pos, QWORD count, void *pBuf, LPARAM par);

class AssetsBundleFile
{
	//AssetsFileReader reader;
	public:
		union {
			AssetsBundleHeader03 bundleHeader3;
			AssetsBundleHeader06 bundleHeader6;
		};
		union {
			AssetsList *assetsLists3;
			AssetsBundleBlockAndDirectoryList06 *bundleInf6;
		};
		DWORD listCount;

		ASSETSTOOLS_API AssetsBundleFile();
		ASSETSTOOLS_API ~AssetsBundleFile();
		ASSETSTOOLS_API void Close();
		ASSETSTOOLS_API bool Read(AssetsFileReader reader, LPARAM lPar, AssetsFileVerifyLogger errorLogger = NULL, bool allowCompressed = false);
		ASSETSTOOLS_API bool Write(AssetsFileReader reader, LPARAM readerPar,
			AssetsFileWriter writer, LPARAM writerPar,
			class BundleReplacer **pReplacers, size_t replacerCount, 
			AssetsFileVerifyLogger errorLogger = NULL, ClassDatabaseFile *typeMeta = NULL);
		ASSETSTOOLS_API bool Unpack(AssetsFileReader reader, LPARAM lPar, AssetsFileWriter writer, LPARAM writerPar);
		ASSETSTOOLS_API bool Pack(AssetsFileReader reader, LPARAM lPar, AssetsFileWriter writer, LPARAM writerPar);
		ASSETSTOOLS_API bool IsAssetsFile(AssetsFileReader reader, LPARAM pLPar, AssetsBundleDirectoryInfo06 *pEntry);
		ASSETSTOOLS_API bool IsAssetsFile(AssetsFileReader reader, LPARAM pLPar, AssetsBundleEntry *pEntry);
		ASSETSTOOLS_API AssetsFileReader MakeAssetsFileReader(AssetsFileReader reader, LPARAM *pLPar, AssetsBundleDirectoryInfo06 *pEntry);
		ASSETSTOOLS_API AssetsFileReader MakeAssetsFileReader(AssetsFileReader reader, LPARAM *pLPar, AssetsBundleEntry *pEntry);
		//void FreeAssetsFileReader(LPARAM *pLPar, AssetsFileReader *pReader);
};
ASSETSTOOLS_API void FreeAssetsBundle_FileReader(LPARAM *pLPar, AssetsFileReader *pReader);

#endif