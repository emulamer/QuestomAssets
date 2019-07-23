using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.Mods
{
    public static class ModExtensions
    {
        public static bool IsExclusiveMod(this ModCategory category)
        {
            var cat = typeof(ModCategory).GetMember(category.ToString());
            var valmem = cat.FirstOrDefault(x => x.DeclaringType == typeof(ModCategory));
            return valmem.GetCustomAttributes(typeof(ExclusiveModAttribute), false).Any();
        }
    }
}
