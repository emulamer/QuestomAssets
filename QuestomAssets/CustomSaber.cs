using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public class CustomSaber
    {
        public bool IsStockSaber { get; private set; }

        public string SaberBladeDatFile { get; set; }

        public string SaberGlowingEdgesDatFile { get; set; }

        public string SaberHandleDatFile { get; set; }

        public static CustomSaber StockSaber { get; } = new CustomSaber() { IsStockSaber = true };
    }
}
