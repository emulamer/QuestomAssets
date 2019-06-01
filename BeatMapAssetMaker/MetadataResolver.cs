using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;

namespace BeatmapAssetMaker
{
    public class MetadataResolver : DefaultContractResolver
    {
        private AssetsChanger.AssetsFile _assetsFile;
        public MetadataResolver(AssetsChanger.AssetsFile assetsFile)
        {
            _assetsFile = assetsFile;
        }
        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            if (typeof(AssetsChanger.INeedAssetsMetadata).IsAssignableFrom(objectType))
            {
                var contract = base.CreateObjectContract(objectType);
                contract.DefaultCreator = () => Activator.CreateInstance(objectType, _assetsFile.Metadata);
                return contract;
            }

            return base.CreateObjectContract(objectType);
        }
    }

}
