using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class AssetActionTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ModComponent);
        }

        public override bool CanWrite { get { return false; } }

        private static Dictionary<AssetActionType, Type> _classMap = new Dictionary<AssetActionType, Type>()
        { {AssetActionType.ReplaceAsset, typeof(ReplaceAssetAction)} };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var typeToken = token["Type"];
            if (typeToken == null)
                throw new InvalidOperationException("Bad asset action, has no type.");
            var actualType = _classMap[typeToken.ToObject<AssetActionType>(serializer)];
            if (existingValue == null || existingValue.GetType() != actualType)
            {
                var contract = serializer.ContractResolver.ResolveContract(actualType);
                existingValue = contract.DefaultCreator();
            }
            using (var subReader = token.CreateReader())
            {
                serializer.Populate(subReader, existingValue);
            }
            return existingValue;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
