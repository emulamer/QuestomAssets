using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public class ImportException : Exception
    {
        public string FriendlyMessage { get; private set; }

        public ImportException(string message, string friendlyMessage, Exception innerException) : base(message, innerException)
        {
            FriendlyMessage = friendlyMessage;
        }

        public ImportException(string message, string friendlyMessage) : base(message)
        {
            FriendlyMessage = friendlyMessage;
        }
    }
}
