#pragma once
#include <string>
#include <iostream>
#include <Windows.h>
#include <fstream>

#ifndef POINTERTYPE
typedef unsigned __int64 QWORD;
#ifdef WIN64
#define POINTERTYPE QWORD
#else
#define POINTERTYPE DWORD
#endif
#endif

#ifdef ASSETSTOOLS_EXPORTS
#define ASSETSTOOLS_API __declspec(dllexport) 
#else
#define ASSETSTOOLS_API __declspec(dllimport) 
typedef unsigned __int64 QWORD;
#endif

#ifndef __AssetsTools_AssetsFileFunctions_Read
#define __AssetsTools_AssetsFileFunctions_Read
typedef QWORD(_cdecl *AssetsFileReader)(QWORD pos, QWORD count, void *pBuf, LPARAM par);
typedef void(_cdecl *AssetsFileVerifyLogger)(char *message);
#endif
#ifndef __AssetsTools_AssetsFileFunctions_Write
#define __AssetsTools_AssetsFileFunctions_Write
typedef QWORD(_cdecl *AssetsFileWriter)(QWORD pos, QWORD count, const void *pBuf, LPARAM par);
#endif

#ifndef __AssetsTools_AssetsReplacerFunctions_FreeCallback
#define __AssetsTools_AssetsReplacerFunctions_FreeCallback
typedef void(_cdecl *cbFreeMemoryResource)(void *pResource);
typedef void(_cdecl *cbFreeReaderResource)(AssetsFileReader reader, LPARAM readerPar);
#endif
#ifndef __AssetsTools_Hash128
#define __AssetsTools_Hash128
union Hash128
{
	BYTE bValue[16];
	WORD wValue[8];
	DWORD dValue[4];
	QWORD qValue[2];
};
#endif