using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.IO.Compression;

// Token: 0x0200019A RID: 410
[Serializable]
public class BeatmapSaveData
{
	// Token: 0x170000FC RID: 252
	// (get) Token: 0x060006A4 RID: 1700 RVA: 0x00018787 File Offset: 0x00016987
	public string version
	{
		get
		{
			return this._version;
		}
	}

	// Token: 0x170000FD RID: 253
	// (get) Token: 0x060006A5 RID: 1701 RVA: 0x0001878F File Offset: 0x0001698F
	public List<BeatmapSaveData.EventData> events
	{
		get
		{
			return this._events;
		}
	}

	// Token: 0x170000FE RID: 254
	// (get) Token: 0x060006A6 RID: 1702 RVA: 0x00018797 File Offset: 0x00016997
	public List<BeatmapSaveData.NoteData> notes
	{
		get
		{
			return this._notes;
		}
	}

	// Token: 0x170000FF RID: 255
	// (get) Token: 0x060006A7 RID: 1703 RVA: 0x0001879F File Offset: 0x0001699F
	public List<BeatmapSaveData.ObstacleData> obstacles
	{
		get
		{
			return this._obstacles;
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x000187A7 File Offset: 0x000169A7
	public BeatmapSaveData(List<BeatmapSaveData.EventData> events, List<BeatmapSaveData.NoteData> notes, List<BeatmapSaveData.ObstacleData> obstacles)
	{
		this._version = "2.0.0";
		this._events = events;
		this._notes = notes;
		this._obstacles = obstacles;
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000187D0 File Offset: 0x000169D0
	public virtual byte[] SerializeToBinary()
	{
		byte[] result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
            using (DeflateStream ds = new DeflateStream(memoryStream, CompressionMode.Compress))
            {
                new BinaryFormatter().Serialize(ds, this);
                ds.Flush();
            }
            memoryStream.Close();
            result = memoryStream.ToArray();
        }
		return result;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x0001881C File Offset: 0x00016A1C
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

	// Token: 0x060006AB RID: 1707 RVA: 0x00018860 File Offset: 0x00016A60
	public virtual string SerializeToJSONString()
	{
		return JsonConvert.SerializeObject(this);
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x00018868 File Offset: 0x00016A68
	public static BeatmapSaveData DeserializeFromJSONString(string stringData)
	{
		BeatmapSaveData beatmapSaveData = JsonConvert.DeserializeObject<BeatmapSaveData>(stringData);
		if (beatmapSaveData != null)
		{
			//beatmapSaveData.version != "2.0.0";
		}
		return beatmapSaveData;
	}

	// Token: 0x040005DE RID: 1502
	protected const string kCurrentVersion = "2.0.0";

	// Token: 0x040005DF RID: 1503
	[JsonProperty]
	protected string _version;

	// Token: 0x040005E0 RID: 1504
	[JsonProperty]
	protected List<BeatmapSaveData.EventData> _events;

	// Token: 0x040005E1 RID: 1505
	[JsonProperty]
	protected List<BeatmapSaveData.NoteData> _notes;

	// Token: 0x040005E2 RID: 1506
	[JsonProperty]
	protected List<BeatmapSaveData.ObstacleData> _obstacles;

	// Token: 0x0200019B RID: 411
	public interface ITime
	{
		// Token: 0x17000100 RID: 256
		// (get) Token: 0x060006AD RID: 1709
		float time { get; }

		// Token: 0x060006AE RID: 1710
		void MoveTime(float offset);
	}

	// Token: 0x0200019C RID: 412
	[Serializable]
	public class EventData : BeatmapSaveData.ITime
	{
		// Token: 0x17000101 RID: 257
		// (get) Token: 0x060006AF RID: 1711 RVA: 0x00018891 File Offset: 0x00016A91
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

		// Token: 0x17000102 RID: 258
		// (get) Token: 0x060006B0 RID: 1712 RVA: 0x00018899 File Offset: 0x00016A99
		public BeatmapEventType type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x17000103 RID: 259
		// (get) Token: 0x060006B1 RID: 1713 RVA: 0x000188A1 File Offset: 0x00016AA1
		public int value
		{
			get
			{
				return this._value;
			}
		}

		// Token: 0x060006B2 RID: 1714 RVA: 0x000188A9 File Offset: 0x00016AA9
		public EventData(float time, BeatmapEventType type, int value)
		{
			this._time = time;
			this._type = type;
			this._value = value;
		}

		// Token: 0x060006B3 RID: 1715 RVA: 0x000188C6 File Offset: 0x00016AC6
		public void MoveTime(float offset)
		{
			this._time += offset;
		}

		// Token: 0x040005E3 RID: 1507
		[JsonProperty]
		protected float _time;

		// Token: 0x040005E4 RID: 1508
		[JsonProperty]
		protected BeatmapEventType _type;

		// Token: 0x040005E5 RID: 1509
		[JsonProperty]
		protected int _value;
	}

	// Token: 0x0200019D RID: 413
	[Serializable]
	public class NoteData : BeatmapSaveData.ITime
	{
		// Token: 0x17000104 RID: 260
		// (get) Token: 0x060006B4 RID: 1716 RVA: 0x000188D6 File Offset: 0x00016AD6
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

		// Token: 0x17000105 RID: 261
		// (get) Token: 0x060006B5 RID: 1717 RVA: 0x000188DE File Offset: 0x00016ADE
		public int lineIndex
		{
			get
			{
				return this._lineIndex;
			}
		}

		// Token: 0x17000106 RID: 262
		// (get) Token: 0x060006B6 RID: 1718 RVA: 0x000188E6 File Offset: 0x00016AE6
		public NoteLineLayer lineLayer
		{
			get
			{
				return this._lineLayer;
			}
		}

		// Token: 0x17000107 RID: 263
		// (get) Token: 0x060006B7 RID: 1719 RVA: 0x000188EE File Offset: 0x00016AEE
		public NoteType type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x17000108 RID: 264
		// (get) Token: 0x060006B8 RID: 1720 RVA: 0x000188F6 File Offset: 0x00016AF6
		public NoteCutDirection cutDirection
		{
			get
			{
				return this._cutDirection;
			}
		}

		// Token: 0x060006B9 RID: 1721 RVA: 0x000188FE File Offset: 0x00016AFE
		public NoteData(float time, int lineIndex, NoteLineLayer lineLayer, NoteType type, NoteCutDirection cutDirection)
		{
			this._time = time;
			this._lineIndex = lineIndex;
			this._lineLayer = lineLayer;
			this._type = type;
			this._cutDirection = cutDirection;
		}

		// Token: 0x060006BA RID: 1722 RVA: 0x0001892B File Offset: 0x00016B2B
		public void MoveTime(float offset)
		{
			this._time += offset;
		}

		// Token: 0x040005E6 RID: 1510
		[JsonProperty]
		protected float _time;

		// Token: 0x040005E7 RID: 1511
		[JsonProperty]
		protected int _lineIndex;

		// Token: 0x040005E8 RID: 1512
		[JsonProperty]
		protected NoteLineLayer _lineLayer;

		// Token: 0x040005E9 RID: 1513
		[JsonProperty]
		protected NoteType _type;

		// Token: 0x040005EA RID: 1514
		[JsonProperty]
		protected NoteCutDirection _cutDirection;
	}

	// Token: 0x0200019E RID: 414
	[Serializable]
	public class ObstacleData : BeatmapSaveData.ITime
	{
		// Token: 0x17000109 RID: 265
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0001893B File Offset: 0x00016B3B
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

		// Token: 0x1700010A RID: 266
		// (get) Token: 0x060006BC RID: 1724 RVA: 0x00018943 File Offset: 0x00016B43
		public int lineIndex
		{
			get
			{
				return this._lineIndex;
			}
		}

		// Token: 0x1700010B RID: 267
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x0001894B File Offset: 0x00016B4B
		public ObstacleType type
		{
			get
			{
				return this._type;
			}
		}

		// Token: 0x1700010C RID: 268
		// (get) Token: 0x060006BE RID: 1726 RVA: 0x00018953 File Offset: 0x00016B53
		public float duration
		{
			get
			{
				return this._duration;
			}
		}

		// Token: 0x1700010D RID: 269
		// (get) Token: 0x060006BF RID: 1727 RVA: 0x0001895B File Offset: 0x00016B5B
		public int width
		{
			get
			{
				return this._width;
			}
		}

		// Token: 0x060006C0 RID: 1728 RVA: 0x00018963 File Offset: 0x00016B63
		public ObstacleData(float time, int lineIndex, ObstacleType type, float duration, int width)
		{
			this._time = time;
			this._lineIndex = lineIndex;
			this._type = type;
			this._duration = duration;
			this._width = width;
		}

		// Token: 0x060006C1 RID: 1729 RVA: 0x00018990 File Offset: 0x00016B90
		public void MoveTime(float offset)
		{
			this._time += offset;
		}

		// Token: 0x040005EB RID: 1515
		[JsonProperty]
		protected float _time;

		// Token: 0x040005EC RID: 1516
		[JsonProperty]
		protected int _lineIndex;

		// Token: 0x040005ED RID: 1517
		[JsonProperty]
		protected ObstacleType _type;

		// Token: 0x040005EE RID: 1518
		[JsonProperty]
		protected float _duration;

		// Token: 0x040005EF RID: 1519
		[JsonProperty]
		protected int _width;
	}
}
