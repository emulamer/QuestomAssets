#pragma once

#include "defines.h"

//Creates a reader that can read split .assets files (opens all at once)
ASSETSTOOLS_API LPARAM Create_AssetsReaderFromSplitFile(const char *baseFileName);
ASSETSTOOLS_API LPARAM Create_AssetsReaderFromSplitFileW(const wchar_t *baseFileName);
ASSETSTOOLS_API void Free_AssetsReaderFromSplitFile(LPARAM lParam);
ASSETSTOOLS_API QWORD AssetsReaderFromSplitFile(QWORD pos, QWORD count, void *pBuf, LPARAM par);

ASSETSTOOLS_API QWORD AssetsReaderFromFile(QWORD pos, QWORD count, void *pBuf, LPARAM par);
ASSETSTOOLS_API void AssetsVerifyLoggerFromFile(char *message);
ASSETSTOOLS_API void AssetsVerifyLoggerToConsole(char *message);

ASSETSTOOLS_API LPARAM Create_AssetsReaderFromMemory(void *buf, size_t bufLen, bool copyBuf);
ASSETSTOOLS_API void Free_AssetsReaderFromMemory(LPARAM lParam, bool freeBuf = false);
ASSETSTOOLS_API QWORD AssetsReaderFromMemory(QWORD pos, QWORD count, void *pBuf, LPARAM par);

//Creates a reader that begins at a specific offset from another reader
ASSETSTOOLS_API LPARAM Create_AssetsWriterOffset(AssetsFileWriter origWriter, LPARAM origPar, QWORD offset);

//Creates a reader that has a specific region of another reader
ASSETSTOOLS_API AssetsFileReader Create_PartialAssetsFileReader(AssetsFileReader reader, LPARAM *pLPar,
	QWORD rangeBegin, QWORD rangeLength);
ASSETSTOOLS_API QWORD PartialAssetsFileReader(QWORD pos, QWORD count, void *pBuf, LPARAM par);
ASSETSTOOLS_API void Free_PartialAssetsFileReader(LPARAM lParam);
ASSETSTOOLS_API AssetsFileReader Free_PartialAssetsFileReader(LPARAM *pLParam);

ASSETSTOOLS_API QWORD AssetsWriterToFile(QWORD pos, QWORD count, const void *pBuf, LPARAM par);

ASSETSTOOLS_API LPARAM Create_AssetsWriterToMemory(void *buf, size_t bufLen);
ASSETSTOOLS_API LPARAM Create_AssetsWriterToMemoryDynamic();
ASSETSTOOLS_API void *Get_AssetsWriterToMemory_Buf(LPARAM lParam, size_t *pPos, size_t *pSize);
ASSETSTOOLS_API void Free_AssetsWriterToMemory(LPARAM lParam, bool freeIfDynamic = true);
ASSETSTOOLS_API void Free_AssetsWriterToMemory_DynMem(void *p);
ASSETSTOOLS_API QWORD AssetsWriterToMemory(QWORD pos, QWORD count, const void *pBuf, LPARAM par);

ASSETSTOOLS_API QWORD AssetsWriterOffset(QWORD pos, QWORD count, const void *pBuf, LPARAM par);
ASSETSTOOLS_API void Free_AssetsWriterOffset(LPARAM lParam);