using QuestomAssets.AssetsChanger;
using QuestomAssets.Mods.Assets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class CreateAssetOp : AssetOp
    {
        public override bool IsWriteOp => true;

        private byte[] _replaceDataWith;
        private AssetType _assetType;
        private string _fileName;
        private int _classID;
        private string _scriptName;
        public CreateAssetOp(byte[] replaceDataWith, AssetType type, string filename, int classID, string scriptName)
        {
            _replaceDataWith = replaceDataWith;
            _fileName = filename;
            _assetType = type;
            _classID = classID;
            _scriptName = scriptName;
        }

        internal override void PerformOp(OpContext context)
        {
            var f = context.Manager.GetAssetsFile(_fileName);
            AssetsObject ao;
            switch (_assetType)
            {
                case AssetType.AudioClip:
                    ao = new AudioClipObject(f);
                    break;
                case AssetType.GameObject:
                    ao = new GameObject(f);
                    break;
                case AssetType.Material:
                    ao = new MaterialObject(f);
                    break;
                case AssetType.Mesh:
                    ao = new MeshObject(f);
                    break;
                case AssetType.MeshFilter:
                    ao = new MeshFilterObject(f);
                    break;
                case AssetType.MonoBehaviour:
                    var msb = context.Manager.GetScriptObject(_scriptName);
                    ao = new MonoBehaviourObject(f, msb);
                    break;
                case AssetType.Sprite:
                    ao = new SpriteObject(f);
                    break;
                case AssetType.Text:
                    ao = new TextAsset(f);
                    break;
                case AssetType.Texture2D:
                    ao = new Texture2DObject(f);
                    break;
                case AssetType.Transform:
                    ao = new Transform(f);
                    break;
                case AssetType.Unknown:
                default:
                    ao = new AssetsObject(f, _classID);
                    break;
            }
            using (var ms = new MemoryStream(_replaceDataWith))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    ao.ObjectInfo.DataOffset = 0;
                    ao.ObjectInfo.DataSize = _replaceDataWith.Length;
                    ao.Parse(reader);

                    ao.ObjectInfo.DataOffset = -1;
                    ao.ObjectInfo.DataSize = -1;
                }
            }
            f.AddObject(ao);
        }
    }
}
