using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;

[Serializable]
public class BeatmapSaveData
{
	public string version
	{
		get
		{
			return this._version;
		}
	}

	public List<BeatmapSaveData.EventData> events
	{
		get
		{
			return this._events;
		}
	}

	public List<BeatmapSaveData.NoteData> notes
	{
		get
		{
			return this._notes;
		}
	}

	public List<BeatmapSaveData.ObstacleData> obstacles
	{
		get
		{
			return this._obstacles;
		}
	}

    public virtual byte[] SerializeToBinary(bool skipDeflate = false)
    {
        byte[] result;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (DeflateStream ds = new DeflateStream(memoryStream, CompressionMode.Compress))
            {
                if (skipDeflate)
                {
                    new BinaryFormatter().Serialize(memoryStream, this);
                }
                else
                {
                    new BinaryFormatter().Serialize(ds, this);
                    ds.Flush();
                }

            }
            memoryStream.Close();
            result = memoryStream.ToArray();
        }
        return result;
    }

    public static BeatmapSaveData DeserializeFromFromBinary(byte[] data)
	{
		BeatmapSaveData result;
        using (MemoryStream memoryStream = new MemoryStream(data))
        {
            using (DeflateStream ds = new DeflateStream(memoryStream, CompressionMode.Decompress))
            {

                result = (BeatmapSaveData)new BinaryFormatter().Deserialize(ds);
            }
		}
        return result;
	}

	public virtual string SerializeToJSONString()
	{
		return JsonConvert.SerializeObject(this);
	}

	public static BeatmapSaveData DeserializeFromJSONString(string stringData)
	{
		BeatmapSaveData beatmapSaveData = JsonConvert.DeserializeObject<BeatmapSaveData>(stringData);
		return beatmapSaveData;
	}

	protected const string kCurrentVersion = "2.0.0";

	[JsonProperty]
	protected string _version;

	[JsonProperty]
	protected List<BeatmapSaveData.EventData> _events;

	[JsonProperty]
	protected List<BeatmapSaveData.NoteData> _notes;

	[JsonProperty]
	protected List<BeatmapSaveData.ObstacleData> _obstacles;


	[Serializable]
	public class EventData 
	{

		public float time
		{
			get
			{
				return this._time;
			}
            set
            {
                this._time = value;
            }
		}

		public BeatmapEventType type
		{
			get
			{
				return this._type;
			}
		}

		public int value
		{
			get
			{
				return this._value;
			}
		}

		public EventData(float time, BeatmapEventType type, int value)
		{
			this._time = time;
			this._type = type;
			this._value = value;
		}

		public void MoveTime(float offset)
		{
			this._time += offset;
		}

		[JsonProperty]
		protected float _time;

		[JsonProperty]
		protected BeatmapEventType _type;

		[JsonProperty]
		protected int _value;
	}

	[Serializable]
	public class NoteData 
	{

		public float time
		{
			get
			{
				return this._time;
			}
            set
            {
                this._time = value;
            }
		}

		public int lineIndex
		{
			get
			{
				return this._lineIndex;
			}
		}

		public NoteLineLayer lineLayer
		{
			get
			{
				return this._lineLayer;
			}
		}

		public NoteType type
		{
			get
			{
				return this._type;
			}
		}

		public NoteCutDirection cutDirection
		{
			get
			{
				return this._cutDirection;
			}
		}

		public void MoveTime(float offset)
		{
			this._time += offset;
		}

		[JsonProperty]
		protected float _time;

		[JsonProperty]
		protected int _lineIndex;

		[JsonProperty]
		protected NoteLineLayer _lineLayer;

		[JsonProperty]
		protected NoteType _type;

		[JsonProperty]
		protected NoteCutDirection _cutDirection;
	}

	[Serializable]
	public class ObstacleData
	{

		public float time
		{
			get
			{
				return this._time;
			}
            set
            {
                this._time = value;
            }
		}
        
		public int lineIndex
		{
			get
			{
				return this._lineIndex;
			}
		}


		public ObstacleType type
		{
			get
			{
				return this._type;
			}
		}

		public float duration
		{
			get
			{
				return this._duration;
			}
		}

		public int width
		{
			get
			{
				return this._width;
			}
		}

        [JsonProperty]
		protected float _time;

		[JsonProperty]
		protected int _lineIndex;

		[JsonProperty]
		protected ObstacleType _type;

		[JsonProperty]
		protected float _duration;

		[JsonProperty]
		protected int _width;
	}
}
