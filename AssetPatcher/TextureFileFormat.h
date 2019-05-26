#pragma once

#include "AssetTypeClass.h"

#ifdef ASSETSTOOLS_EXPORTS
#define ASSETSTOOLS_API __declspec(dllexport) 
#else
#define ASSETSTOOLS_API __declspec(dllimport) 
#endif

#define MinTextureFileSize 0x38
struct TextureFile
{
	char *m_Name;
	unsigned int m_Width;			//0x00
	unsigned int m_Height;			//0x04
	DWORD m_CompleteImageSize;		//0x08
	DWORD m_TextureFormat;			//0x0C ; TextureFormat
	int m_MipCount;					//added with 5.2.x //0x10
	bool m_MipMap;					//0x10 or non-existant
	bool m_IsReadable;				//0x11 or 0x14
	bool m_ReadAllowed;				//0x12 or 0x15 or non-existant
	bool padding1;					//0x13 or 0x16 or ...
	int m_ImageCount;				//0x14 or 0x18
	int m_TextureDimension;			//0x18 or 0x1C; Flags : 0x40, 0x10, 0x02, 0x01
	struct GLTextureSettings
	{
		int m_FilterMode;			//0x1C or 0x20 ; FilterMode : Point, Bilinear, Trilinear
		int m_Aniso;				//0x20 or 0x24; AnisotropicFiltering : Disable,Enable,ForceEnable
		float m_MipBias;			//0x24 or 0x28
		int m_WrapMode;				//0x28 or 0x2C; TextureWrapMode : Repeat, Clamp
	} m_TextureSettings;
	int m_LightmapFormat;			//0x2C or 0x30; LightmapsMode(?) : NonDirectional, CombinedDirectional, SeparateDirectional
	int m_ColorSpace;				//0x30 or 0x34; ColorSpace : Gamma, Linear
	DWORD _pictureDataSize;			//0x34 or 0x38 //the same as pictureDataSize
	BYTE *pPictureData;
	struct StreamingInfo			//0x38 + _pictureDataSize
	{
		unsigned int offset;
		unsigned int size;
		char *path;
	} m_StreamData;
	BYTE pictureData[0];			//0x38 or 0x3C; actually before m_StreamData
};
enum TextureFormat { //by disunity and UnityEngine.dll
	TexFmt_Alpha8=1,
	TexFmt_ARGB4444,
	TexFmt_RGB24,
	TexFmt_RGBA32,
	TexFmt_ARGB32,
	TexFmt_UNUSED06,
	TexFmt_RGB565,
	TexFmt_UNUSED08,
	TexFmt_R16,
	TexFmt_DXT1,
	TexFmt_UNUSED11,
	TexFmt_DXT5,
	TexFmt_RGBA4444,
	TexFmt_BGRA32New, //Unity 5
	TexFmt_RHalf,
	TexFmt_RGHalf,
	TexFmt_RGBAHalf,
	TexFmt_RFloat,
	TexFmt_RGFloat,
	TexFmt_RGBAFloat,
	TexFmt_YUV2,
	TexFmt_UNUSED22,
	TexFmt_UNUSED23,
	TexFmt_BC6H, //Unity 5.5
	TexFmt_BC7, //Unity 5.5
	TexFmt_BC4, //Unity 5.5
	TexFmt_BC5, //Unity 5.5
	TexFmt_DXT1Crunched,
	TexFmt_DXT5Crunched,
	TexFmt_PVRTC_RGB2,
	TexFmt_PVRTC_RGBA2,
	TexFmt_PVRTC_RGB4,
	TexFmt_PVRTC_RGBA4,
	TexFmt_ETC_RGB4,
	TexFmt_ATC_RGB4,
	TexFmt_ATC_RGBA8,
	TexFmt_BGRA32Old, //Unity 4
	TexFMT_UNUSED38, //TexFmt_ATF_RGB_DXT1,
	TexFMT_UNUSED39, //TexFmt_ATF_RGBA_JPG,
	TexFMT_UNUSED40, //TexFmt_ATF_RGB_JPG,
	TexFmt_EAC_R,
	TexFmt_EAC_R_SIGNED,
	TexFmt_EAC_RG,
	TexFmt_EAC_RG_SIGNED,
	TexFmt_ETC2_RGB4,
	TexFmt_ETC2_RGBA1,
	TexFmt_ETC2_RGBA8,
	TexFmt_ASTC_RGB_4x4,
	TexFmt_ASTC_RGB_5x5,
	TexFmt_ASTC_RGB_6x6,
	TexFmt_ASTC_RGB_8x8,
	TexFmt_ASTC_RGB_10x10,
	TexFmt_ASTC_RGB_12x12,
	TexFmt_ASTC_RGBA_4x4,
	TexFmt_ASTC_RGBA_5x5,
	TexFmt_ASTC_RGBA_6x6,
	TexFmt_ASTC_RGBA_8x8,
	TexFmt_ASTC_RGBA_10x10,
	TexFmt_ASTC_RGBA_12x12,
	TexFmt_ETC_RGB4_3DS,
	TexFmt_ETC_RGBA8_3DS
};

ASSETSTOOLS_API bool ReadTextureFile(TextureFile *pOutTex, AssetTypeValueField *pBaseField);
ASSETSTOOLS_API QWORD WriteTextureFile(TextureFile *pInTex, AssetTypeTemplateField *pBaseTemplate, AssetsFileWriter writer, LPARAM writerPar);
ASSETSTOOLS_API bool GetTextureData(TextureFile *pTex, void *pOutBuf);
ASSETSTOOLS_API size_t GetCompressedTextureDataSize(int width, int height, TextureFormat texFmt);
ASSETSTOOLS_API unsigned int GetCompressedTextureDataSizeCrunch(TextureFile *pTex);
ASSETSTOOLS_API bool MakeTextureData(TextureFile *pTex, void *pRGBA32Buf, bool rotate180, int compressQuality = 0);