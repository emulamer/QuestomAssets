using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QuestomAssets.Mods

{
    public class ModComponentTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(ModComponent);
        }

        public override bool CanWrite { get { return false; } }

        private static Dictionary<ModComponentType, Type> _classMap = new Dictionary<ModComponentType, Type>()
        { {ModComponentType.HookMod, typeof(HookModComponent)},
            {ModComponentType.AssetsMod, typeof(Assets.AssetsModComponent) },
            {ModComponentType.FileCopyMod, typeof(FileCopyModComponent) }
            };

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var typeToken = token["Type"];
            if (typeToken == null)
                throw new InvalidOperationException("Bad component, has no type.");
            var actualType = _classMap[typeToken.ToObject<ModComponentType>(serializer)];
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