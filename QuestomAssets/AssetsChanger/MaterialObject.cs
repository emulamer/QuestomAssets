using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class MaterialObject : AssetsObject, IHaveName
    {
        public string Name { get; set; }
        public SmartPtr<AssetsObject> Shader { get; set; }
        public string ShaderKeywords { get; set; }
        public UInt32 LightmapFlags { get; set; }
        public bool EnableInstancingVariants { get; set; }
        public bool DoubleSidedGI { get; set; }
        public int CustomRenderQueue { get; set; }
        public List<Map<string, string>> StringTagMap { get; set; } = new List<Map<string, string>>();
        public List<string> DisabledShaderPasses { get; set; } = new List<string>();
        public PropertySheet SavedProperties { get; set; }

        public MaterialObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.MeshFilterClassID)
        {
        }

        public MaterialObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Name = reader.ReadString();
            Shader = SmartPtr<AssetsObject>.Read(this.ObjectInfo.ParentFile, this, reader);
            ShaderKeywords = reader.ReadString();
            LightmapFlags = reader.ReadUInt32();
            EnableInstancingVariants = reader.ReadBoolean();
            DoubleSidedGI = reader.ReadBoolean();
            reader.AlignTo(4);
            CustomRenderQueue = reader.ReadInt32();
            StringTagMap = reader.ReadArrayOf(r => new Map<string, string>(reader.ReadString(), reader.ReadString()));
            DisabledShaderPasses = reader.ReadArrayOf(r => r.ReadString());
            SavedProperties = new PropertySheet(this.ObjectInfo.ParentFile, this, reader);            
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            Shader.Write(writer);
            writer.Write(ShaderKeywords);
            writer.Write(LightmapFlags);
            writer.Write(EnableInstancingVariants);
            writer.Write(DoubleSidedGI);
            writer.AlignTo(4);
            writer.Write(CustomRenderQueue);
            writer.WriteArrayOf(StringTagMap, (o, w) =>
            {
                w.Write(o.First);
                w.Write(o.Second);
            });
            writer.WriteArrayOf(DisabledShaderPasses, (o, w) =>
            {
                w.Write(o);
            });
            SavedProperties.Write(writer);            
        }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }

    }
}
