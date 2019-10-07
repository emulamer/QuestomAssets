using QuestomAssets.AssetsChanger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class AssetLocator
    {
        /// <summary>
        /// If specified, filters by a specific type of asset
        /// </summary>
        public AssetType? TypeIs { get; set; }

        /// <summary>
        /// If specified, filters by named assets with a specific name (case sensitive)
        /// </summary>
        public string NameIs { get; set; }

        /// <summary>
        /// If specified, filters by a specific file/path.  Generally not used in combination with others, and also not version stable
        /// </summary>
        public PathLocator PathIs { get; set; }


        public AssetsObject Locate(AssetsManager manager, bool forceDeepSearch = false)
        {
            List<Func<IObjectInfo<AssetsObject>, bool>> filters = new List<Func<IObjectInfo<AssetsObject>, bool>>();

            if (TypeIs.HasValue)
            {
                switch (TypeIs.Value)
                {
                    case AssetType.AudioClip:
                        filters.Add(x => typeof(IObjectInfo<AudioClipObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Component:
                        filters.Add(x => typeof(IObjectInfo<Component>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.GameObject:
                        filters.Add(x => typeof(IObjectInfo<GameObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Mesh:
                        filters.Add(x => typeof(IObjectInfo<MeshObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.MeshFilter:
                        filters.Add(x => typeof(IObjectInfo<MeshFilterObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.MonoBehaviour:
                        filters.Add(x => typeof(IObjectInfo<MonoBehaviourObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Sprite:
                        filters.Add(x => typeof(IObjectInfo<SpriteObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Text:
                        filters.Add(x => typeof(IObjectInfo<TextAsset>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Texture2D:
                        filters.Add(x => typeof(IObjectInfo<Texture2DObject>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Transform:
                        filters.Add(x => typeof(IObjectInfo<Transform>).IsAssignableFrom(x.GetType()));
                        break;
                    case AssetType.Material:
                        filters.Add(x => typeof(IObjectInfo<MaterialObject>).IsAssignableFrom(x.GetType()));
                        break;
                    default:
                        throw new ArgumentException($"Unhandled type value {TypeIs.Value.ToString()}");
                }
            }
            if (NameIs != null)
            {
                filters.Add(x => typeof(IObjectInfo<IHaveName>).IsAssignableFrom(x.GetType()) && (x.Object as IHaveName)?.Name == NameIs);
            }

            if (PathIs != null)
            {
                if (PathIs.AssetFilename == null)
                    throw new ArgumentException("AssetFilename must be specified when using PathIs locator.");
                filters.Add(x => x.ParentFile.AssetsFilename == PathIs.AssetFilename && x.ObjectID == PathIs.PathID);
            }

            try
            {
                var found = manager.MassFindAssets<AssetsObject>(x => !filters.Any(y => !y(x)), forceDeepSearch).SingleOrDefault()?.Object;
                if (found == null)
                    throw new LocatorException("Locator did not find any assets");
                return found;
            }
            catch (Exception ex)
            {
                throw new LocatorException("The locator throw an exception, possibly because it returned more than one matching asset.", ex);
            }
            
        }

    }
}
