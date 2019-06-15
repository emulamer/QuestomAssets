using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestomAssets;

namespace BeatOnLib
{
    public class StringLogger : ILog
    {
        private StringBuilder builder = new StringBuilder();

        public void LogErr(string message, Exception ex)
        {
            builder.Append($"{message}: {ex.Message} {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                LogErr("\tInner exception", ex.InnerException);
            }
            builder.Append("\r\n");
        }

        public void ClearLog()
        {
            builder.Clear();
        }

        public string GetString()
        {
            return builder.ToString();
        }

        public void LogErr(string message, params object[] args)
        {
            builder.Append(string.Format(message, args));
            builder.Append("\r\n");
        }

        public void LogMsg(string message, params object[] args)
        {
            builder.Append(string.Format(message, args));
            builder.Append("\r\n");
        }
    }
}