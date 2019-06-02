using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets
{
    public interface ILog
    {
        void LogMsg(string message, params object[] args);

        void LogErr(string message, Exception ex);

        void LogErr(string message, params object[] args);
    }
}
