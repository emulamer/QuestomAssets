using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public sealed class AssetBundle : AssetsObject, IHaveName
    {
        public AssetBundle(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MonoScriptType)
        {
        }

        public AssetBundle(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Name = reader.ReadString();
            PreloadTable = reader.ReadArrayOf<ISmartPtr<AssetsObject>>(x => SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, x));
            Container = reader.ReadArrayOf(x => new BundleMap(ObjectInfo.ParentFile, this, x));
            MainAsset = new AssetInfo(ObjectInfo.ParentFile, this, reader);
            RuntimeCompatibility = reader.ReadUInt32();
            AssetBundleName = reader.ReadString();
            Dependencies = reader.ReadArrayOf<ISmartPtr<AssetsObject>>(x => SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, x));
            IsStreamedSceneAssetBundle = reader.ReadBoolean();
            reader.AlignTo(4);
            ExplicitDataLayout = reader.ReadInt32();
            PathFlags = reader.ReadInt32();
            SceneHashes = reader.ReadArrayOf(x => new BundleMap(ObjectInfo.ParentFile, this, x));
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.WriteArrayOf(PreloadTable, (x , y) => x.WritePtr(y));
            writer.WriteArrayOf(Container, (x, y) => x.Write(y));
            MainAsset.Write(writer);
            writer.Write(RuntimeCompatibility);
            writer.Write(AssetBundleName);
            writer.WriteArrayOf(Dependencies, (x, y) => x.WritePtr(y));
            writer.Write(IsStreamedSceneAssetBundle);
            writer.AlignTo(4);
            writer.Write(ExplicitDataLayout);
            writer.Write(PathFlags);
            writer.WriteArrayOf(SceneHashes, (x, y) => x.Write(y));
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public string Name { get; set; }
        public List<ISmartPtr<AssetsObject>> PreloadTable { get; set; } = new List<ISmartPtr<AssetsObject>>();
        public List<BundleMap> Container { get; set; } = new List<BundleMap>();
        public AssetInfo MainAsset { get; set; }
        public UInt32 RuntimeCompatibility { get; set; }
        public string AssetBundleName { get; set; }

        //TODO: I don't know what type "Dependencies" actually is... I think it's a pointer?
        public List<ISmartPtr<AssetsObject>> Dependencies { get; set; } = new List<ISmartPtr<AssetsObject>>();
        public bool IsStreamedSceneAssetBundle { get; set; }
        public Int32 ExplicitDataLayout { get; set; }
        public Int32 PathFlags { get; set; }
        public List<BundleMap> SceneHashes { get; set; }

    }
}
