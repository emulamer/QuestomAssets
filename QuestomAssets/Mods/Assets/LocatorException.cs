using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.Mods.Assets
{
    public class LocatorException : Exception
    {
        public LocatorException(string message) : base(message)
        {
        }

        public LocatorException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
