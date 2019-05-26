#pragma once
#include "defines.h"

enum AssetsReplacementType
{
	AssetsReplacement_AddOrModify,
	AssetsReplacement_Remove
};
class AssetsReplacer
{
	public:
		virtual AssetsReplacementType GetType() = 0;
		virtual ~AssetsReplacer()
			#ifndef ASSETSTOOLS_EXPORTS
			= 0
			#endif
			;

		virtual DWORD GetFileID() = 0;
		virtual QWORD GetPathID() = 0;
		virtual int GetClassID() = 0;
		virtual WORD GetMonoScriptID() = 0;
		//For add and modify
		virtual QWORD GetSize() = 0;
		virtual QWORD Write(QWORD pos, AssetsFileWriter writer, LPARAM writerPar) = 0;
		//always writes 0 for the file id
		virtual QWORD WriteReplacer(QWORD pos, AssetsFileWriter writer, LPARAM writerPar) = 0;
};

ASSETSTOOLS_API AssetsReplacer *ReadAssetsReplacer(QWORD &pos, AssetsFileReader reader, LPARAM readerPar, bool prefReplacerInMemory = false);
ASSETSTOOLS_API AssetsReplacer *MakeAssetRemover(DWORD fileID, QWORD pathID, int classID, WORD monoScriptIndex = 0xFFFF);
ASSETSTOOLS_API AssetsReplacer *MakeAssetModifierFromReader(DWORD fileID, QWORD pathID, int classID, WORD monoScriptIndex, 
		AssetsFileReader reader, LPARAM readerPar, QWORD size, QWORD readerPos=0, 
		size_t copyBufferLen=0);
ASSETSTOOLS_API AssetsReplacer *MakeAssetModifierFromMemory(DWORD fileID, QWORD pathID, int classID, WORD monoScriptIndex, void *buffer, size_t size, cbFreeMemoryResource freeResourceCallback);
ASSETSTOOLS_API AssetsReplacer *MakeAssetModifierFromFile(DWORD fileID, QWORD pathID, int classID, WORD monoScriptIndex, FILE *pFile, QWORD offs, QWORD size, size_t copyBufferLen=0, bool freeFile=true);
ASSETSTOOLS_API void FreeAssetsReplacer(AssetsReplacer *pReplacer);