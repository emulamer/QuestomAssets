using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BeatmapAssetMaker.AssetsChanger
{
    public class AudioClipObject : AssetsObject
    {
        private string _name;
        private int _loadType;
        private int _channels;
        private int _frequency;
        private int _bitsPerSample;
        private Single _length;
        private bool _isTrackerFormat;
        private bool _ambisonic;
        private int _subsoundIndex;
        private bool _preloadAudioData;
        private bool _loadInBackground;
        private bool _legacy3D;
        private StreamedResource _resource;
        private int _compressionFormat;

        private bool _changes;
        private byte[] _data;

        public string Name { get; set; }

        public int LoadType { get; set; }

        public int Channels { get; set; }

        public int Frequency { get; set; }

        public int BitsPerSample { get; set; }

        public Single Length { get; set; }

        public bool IsTrackerFormat { get; set; }

        public bool Ambisonic { get; set; }

        public int SubsoundIndex { get; set; }

        public bool PreloadAudioData { get; set; }

        public bool LoadInBackground { get; set; }

        public bool Legacy3D { get; set; }

        public StreamedResource Resource { get; set; }

        public int CompressionFormat { get; set; }
        
        public AudioClipObject(ObjectInfo objectInfo) : base(objectInfo)
        { }

        public AudioClipObject(ObjectInfo objectInfo, AssetsReader reader) : base(objectInfo)
        {
            Parse(reader);
        }

        public AudioClipObject(AssetsMetadata metadata) : base(metadata, AssetsConstants.ClassID.AudioClipClassID)
        { }

        protected override void Parse(AssetsReader reader)
        {
            base.Parse(reader);
            Name = reader.ReadString();
            LoadType = reader.ReadInt32();
            Channels = reader.ReadInt32();
            Frequency = reader.ReadInt32();
            BitsPerSample = reader.ReadInt32();
            Length = reader.ReadSingle();
            IsTrackerFormat = reader.ReadBoolean();
            Ambisonic = reader.ReadBoolean();
            reader.AlignTo(4);
            SubsoundIndex = reader.ReadInt32();
            PreloadAudioData = reader.ReadBoolean();
            LoadInBackground = reader.ReadBoolean();
            Legacy3D = reader.ReadBoolean();
            reader.AlignTo(4);
            Resource = new StreamedResource(reader);
            CompressionFormat = reader.ReadInt32();
        }
        public override void Write(AssetsWriter writer)
        {
            base.WriteBase(writer);
            writer.Write(Name);
            writer.Write(LoadType);
            writer.Write(Channels);
            writer.Write(Frequency);
            writer.Write(BitsPerSample);
            writer.Write(Length);
            writer.Write(IsTrackerFormat);
            writer.Write(Ambisonic);
            writer.AlignTo(4);
            writer.Write(SubsoundIndex);
            writer.Write(PreloadAudioData);
            writer.Write(LoadInBackground);
            writer.Write(Legacy3D);
            writer.AlignTo(4);
            Resource.Write(writer);
            writer.Write(CompressionFormat);
        }

        

        public override byte[] Data
        {
            get
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }
            set
            {
                throw new InvalidOperationException("Data cannot be accessed from this class.");
            }


        }
    }
}
