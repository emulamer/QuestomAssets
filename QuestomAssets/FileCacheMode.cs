using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public enum FileCacheMode
    {
        //None won't work for some stuff since it can't seek.
        None,
        Memory,
        TempFiles
    };
}
