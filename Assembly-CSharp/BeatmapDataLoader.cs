using System;
using System.Collections.Generic;


// Token: 0x020001A8 RID: 424
public class BeatmapDataLoader
{
	// Token: 0x060006F0 RID: 1776 RVA: 0x00018F00 File Offset: 0x00017100
	private static float GetRealTimeFromBPMTime(float bmpTime, float beatsPerMinute, float shuffle, float shufflePeriod)
	{
		float num = bmpTime;
		if (shufflePeriod > 0f && (int)(num * (1f / shufflePeriod)) % 2 == 1)
		{
			num += shuffle * shufflePeriod;
		}
		if (beatsPerMinute > 0f)
		{
			num = num / beatsPerMinute * 60f;
		}
		return num;
	}

	// Token: 0x060006F1 RID: 1777 RVA: 0x00018F44 File Offset: 0x00017144
	public static BeatmapData GetBeatmapDataFromBeatmapSaveData(List<BeatmapSaveData.NoteData> notesSaveData, List<BeatmapSaveData.ObstacleData> obstaclesSaveData, List<BeatmapSaveData.EventData> eventsSaveData, float beatsPerMinute, float shuffle, float shufflePeriod)
	{
		List<BeatmapObjectData>[] array = new List<BeatmapObjectData>[4];
		List<BeatmapEventData> list = new List<BeatmapEventData>(eventsSaveData.Count);
		for (int i = 0; i < 4; i++)
		{
			array[i] = new List<BeatmapObjectData>(3000);
		}
		int num = 0;
		NoteData noteData = null;
		float num2 = -1f;
		List<NoteData> list2 = new List<NoteData>(4);
		float num3 = 0f;
		foreach (BeatmapSaveData.NoteData noteData2 in notesSaveData)
		{
			float realTimeFromBPMTime = BeatmapDataLoader.GetRealTimeFromBPMTime(noteData2.time, beatsPerMinute, shuffle, shufflePeriod);
			if (num3 > realTimeFromBPMTime)
			{
				//Debug.LogError("Notes are not ordered.");
			}
			num3 = realTimeFromBPMTime;
			int lineIndex = noteData2.lineIndex;
			NoteLineLayer lineLayer = noteData2.lineLayer;
			NoteLineLayer startNoteLineLayer = NoteLineLayer.Base;
			if (noteData != null && noteData.lineIndex == lineIndex && Math.Abs(noteData.time - realTimeFromBPMTime) < 0.0001f)
			{
				if (noteData.startNoteLineLayer == NoteLineLayer.Base)
				{
					startNoteLineLayer = NoteLineLayer.Upper;
				}
				else
				{
					startNoteLineLayer = NoteLineLayer.Top;
				}
			}
			NoteType type = noteData2.type;
			NoteCutDirection cutDirection = noteData2.cutDirection;
			if (list2.Count > 0 && list2[0].time < realTimeFromBPMTime - 0.001f && type.IsBasicNote())
			{
				BeatmapDataLoader.ProcessBasicNotesInTimeRow(list2, realTimeFromBPMTime);
				num2 = list2[0].time;
				list2.Clear();
			}
			NoteData noteData3 = new NoteData(num++, realTimeFromBPMTime, lineIndex, lineLayer, startNoteLineLayer, type, cutDirection, float.MaxValue, realTimeFromBPMTime - num2);
			array[lineIndex].Add(noteData3);
			noteData = noteData3;
			if (noteData3.noteType.IsBasicNote())
			{
				list2.Add(noteData);
			}
		}
		BeatmapDataLoader.ProcessBasicNotesInTimeRow(list2, float.MaxValue);
		foreach (BeatmapSaveData.ObstacleData obstacleData in obstaclesSaveData)
		{
			float realTimeFromBPMTime2 = BeatmapDataLoader.GetRealTimeFromBPMTime(obstacleData.time, beatsPerMinute, shuffle, shufflePeriod);
			int lineIndex2 = obstacleData.lineIndex;
			ObstacleType type2 = obstacleData.type;
			float realTimeFromBPMTime3 = BeatmapDataLoader.GetRealTimeFromBPMTime(obstacleData.duration, beatsPerMinute, shuffle, shufflePeriod);
			int width = obstacleData.width;
			ObstacleData item = new ObstacleData(num++, realTimeFromBPMTime2, lineIndex2, type2, realTimeFromBPMTime3, width);
			array[lineIndex2].Add(item);
		}
		foreach (BeatmapSaveData.EventData eventData in eventsSaveData)
		{
			float realTimeFromBPMTime4 = BeatmapDataLoader.GetRealTimeFromBPMTime(eventData.time, beatsPerMinute, shuffle, shufflePeriod);
			BeatmapEventType type3 = eventData.type;
			int value = eventData.value;
			BeatmapEventData item2 = new BeatmapEventData(realTimeFromBPMTime4, type3, value);
			list.Add(item2);
		}
		if (list.Count == 0)
		{
			list.Add(new BeatmapEventData(0f, BeatmapEventType.Event0, 1));
			list.Add(new BeatmapEventData(0f, BeatmapEventType.Event4, 1));
		}
		BeatmapLineData[] array2 = new BeatmapLineData[4];
		for (int j = 0; j < 4; j++)
		{
			array[j].Sort(delegate(BeatmapObjectData x, BeatmapObjectData y)
			{
				if (x.time == y.time)
				{
					return 0;
				}
				if (x.time <= y.time)
				{
					return -1;
				}
				return 1;
			});
			array2[j] = new BeatmapLineData();
			array2[j].beatmapObjectsData = array[j].ToArray();
		}
		return new BeatmapData(array2, list.ToArray());
	}

	// Token: 0x060006F2 RID: 1778 RVA: 0x000192AC File Offset: 0x000174AC
	private static void ProcessBasicNotesInTimeRow(List<NoteData> notes, float nextRowTime)
	{
		if (notes.Count == 2)
		{
			NoteData noteData = notes[0];
			NoteData noteData2 = notes[1];
			if (noteData.noteType != noteData2.noteType && ((noteData.noteType == NoteType.NoteA && noteData.lineIndex > noteData2.lineIndex) || (noteData.noteType == NoteType.NoteB && noteData.lineIndex < noteData2.lineIndex)))
			{
				noteData.SetNoteFlipToNote(noteData2);
				noteData2.SetNoteFlipToNote(noteData);
			}
		}
		for (int i = 0; i < notes.Count; i++)
		{
			notes[i].timeToNextBasicNote = nextRowTime - notes[i].time;
		}
	}

	// Token: 0x060006F3 RID: 1779 RVA: 0x00019348 File Offset: 0x00017548
	public static BeatmapData GetBeatmapDataFromBinary(byte[] data, float beatsPerMinute, float shuffle, float shufflePeriod)
	{
		BeatmapSaveData beatmapSaveData = BeatmapSaveData.DeserializeFromFromBinary(data);
		if (beatmapSaveData != null)
		{
			List<BeatmapSaveData.NoteData> notes = beatmapSaveData.notes;
			List<BeatmapSaveData.ObstacleData> obstacles = beatmapSaveData.obstacles;
			List<BeatmapSaveData.EventData> events = beatmapSaveData.events;
			return BeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(notes, obstacles, events, beatsPerMinute, shuffle, shufflePeriod);
		}
		return null;
	}

	// Token: 0x060006F4 RID: 1780 RVA: 0x00019384 File Offset: 0x00017584
	public static BeatmapData GetBeatmapDataFromJson(string json, float beatsPerMinute, float shuffle, float shufflePeriod)
	{
		BeatmapSaveData beatmapSaveData = BeatmapSaveData.DeserializeFromJSONString(json);
		if (beatmapSaveData != null)
		{
			List<BeatmapSaveData.NoteData> notes = beatmapSaveData.notes;
			List<BeatmapSaveData.ObstacleData> obstacles = beatmapSaveData.obstacles;
			List<BeatmapSaveData.EventData> events = beatmapSaveData.events;
			return BeatmapDataLoader.GetBeatmapDataFromBeatmapSaveData(notes, obstacles, events, beatsPerMinute, shuffle, shufflePeriod);
		}
		return null;
	}

    public static BeatmapSaveData GetBeatmapSaveDataFromJson(string json)
    {
        return BeatmapSaveData.DeserializeFromJSONString(json);
        
    }
}
