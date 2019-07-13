using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class Map<T, Y>
    {
        public Map()
        { }

        public Map(T first, Y second)
        {
            First = first;
            Second = second;
        }


        public T First { get; set; }
        public Y Second { get; set; }
    }
}
