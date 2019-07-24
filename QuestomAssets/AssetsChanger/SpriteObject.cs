using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{

    /// <summary>
    /// !!!!!!!!!!!!THIS CLASS IS HORRIBLY INCOMPLETE AND JUST HACKED TOGETHER TO GET TO THE TEXTURE POINTER!!!!!!!!!!
    /// </summary>
    public sealed class SpriteObject : AssetsObject, IHaveName
    {
        public SpriteObject(AssetsFile assetsFile) : base(assetsFile, AssetsConstants.ClassID.SpriteClassID)
        {
        }

        public SpriteObject(IObjectInfo<AssetsObject> objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public override void Parse(AssetsReader reader)
        {
            base.ParseBase(reader);
            Name = reader.ReadString();
            Rect = new RectF(reader);
            Offset = new Vector2F(reader);
            Border = new Vector4F(reader);
            PixelsToUnits = reader.ReadSingle();
            Pivot = new Vector2F(reader);
            Extrude = reader.ReadUInt32();
            IsPolygon = reader.ReadBoolean();
            reader.AlignTo(4);
            RenderDataKey = new Map<Guid, long>(reader.ReadGuid(), reader.ReadInt64());
            AtlasTags = reader.ReadArrayOf(r => r.ReadString());
            SpriteAtlas = SmartPtr<AssetsObject>.Read(ObjectInfo.ParentFile, this, reader);
            RenderData = new SpriteRenderData(ObjectInfo.ParentFile, this, reader);
            PhysicsShape = reader.ReadArrayOf(r => r.ReadArrayOf(r2 => new Vector2F(r2)));
            Bones = reader.ReadArrayOf(r => (ISmartPtr<Transform>)SmartPtr<Transform>.Read(ObjectInfo.ParentFile, this, reader));
        }

        protected override void WriteBase(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            Rect.Write(writer);
            Offset.Write(writer);
            Border.Write(writer);
            writer.Write(PixelsToUnits);
            Pivot.Write(writer);
            writer.Write(Extrude);
            writer.Write(IsPolygon);
            writer.AlignTo(4);
            writer.Write(RenderDataKey.First);
            writer.Write(RenderDataKey.Second);
            writer.WriteArrayOf(AtlasTags, (o, w) => w.Write(o));
            SpriteAtlas.Write(writer);
            RenderData.Write(writer);
            writer.WriteArrayOf(PhysicsShape, (o, w) => w.WriteArrayOf(o, (o2, w2) => o2.Write(w2)));
            writer.WriteArrayOf(Bones, (o, w) => o.Write(w));
        }

        protected override void WriteObject(AssetsWriter writer)
        {
            WriteBase(writer);
        }

        public string Name { get; set; }
        public RectF Rect { get; set; }
        public Vector2F Offset { get; set; }
        public Vector4F Border { get; set; }
        public Single PixelsToUnits { get; set; }
        public Vector2F Pivot { get; set; }
        public UInt32 Extrude { get; set; }
        public bool IsPolygon { get; set; }
        public Map<Guid, Int64> RenderDataKey { get; set; }
        public List<string> AtlasTags { get; set;}        
        //should be SpriteAtlas type, but it isn't mapped yet
        public ISmartPtr<AssetsObject> SpriteAtlas { get; set; }
        public SpriteRenderData RenderData { get; set; }
        public List<List<Vector2F>> PhysicsShape { get; set; }
        public List<ISmartPtr<Transform>> Bones { get; set; }

        [System.ComponentModel.Browsable(false)]
        [Newtonsoft.Json.JsonIgnore]
        public override byte[] Data { get => throw new InvalidOperationException("Data cannot be accessed from this class!"); set => throw new InvalidOperationException("Data cannot be accessed from this class!"); }

        public class SpriteRenderData
        {
            public SpriteRenderData()
            {
            }
            public SpriteRenderData(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
            {
                Parse(assetsFile, owner, reader);
            }
            public void Parse(AssetsFile assetsFile, AssetsObject owner, AssetsReader reader)
            {
                Texture = SmartPtr<Texture2DObject>.Read(assetsFile, owner, reader);
                AlphaTexture = SmartPtr<Texture2DObject>.Read(assetsFile, owner, reader);
                SubMeshes = reader.ReadArrayOf(r => new Submesh(reader));
                IndexBuffer = reader.ReadArray();
                reader.AlignTo(4);
                VertexData = new VertexData(reader);
                BindPose = reader.ReadArrayOf(r => r.ReadSingle());
                TextureRect = new RectF(reader);
                TextureRectOffset = new Vector2F(reader);
                AtlasRectOffset = new Vector2F(reader);
                SettingsRaw = reader.ReadUInt32();
                UVTransform = new Vector4F(reader);
                DownscaleMultiplier = reader.ReadSingle();
            }
            public void Write(AssetsWriter writer)
            {
                Texture.Write(writer);
                AlphaTexture.Write(writer);
                writer.WriteArrayOf(SubMeshes, (o, w) => o.Write(w));
                writer.WriteArray(IndexBuffer);
                writer.AlignTo(4);
                VertexData.Write(writer);
                writer.WriteArrayOf(BindPose, (o, w) => w.Write(o));
                TextureRect.Write(writer);
                TextureRectOffset.Write(writer);
                AtlasRectOffset.Write(writer);
                writer.Write(SettingsRaw);
                UVTransform.Write(writer);
                writer.Write(DownscaleMultiplier);
            }
            public ISmartPtr<Texture2DObject> Texture { get; set; }
            public ISmartPtr<Texture2DObject> AlphaTexture { get; set; }
            public List<Submesh> SubMeshes { get; set; }
            public byte[] IndexBuffer { get; set; }
            public VertexData VertexData { get; set; }
            public List<Single> BindPose { get; set; }
            public RectF TextureRect { get; set; }
            public Vector2F TextureRectOffset { get; set; }
            public Vector2F AtlasRectOffset { get; set; }
            public UInt32 SettingsRaw { get; set; }
            public Vector4F UVTransform { get; set; }
            public Single DownscaleMultiplier { get; set; }

        }
    }
}
