using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class AddSongException : Exception
    {
        public AddSongFailType FailType { get; private set; }
        public AddSongException(AddSongFailType failType, string message, Exception innerException) : base(message, innerException)
        {
            FailType = failType;
        }
        public AddSongException(AddSongFailType failType, string message) : base(message)
        {
            FailType = failType;
        }
    }

    public enum AddSongFailType
    {
        Other,
        InvalidFormat,
        SongExists
    }
}
