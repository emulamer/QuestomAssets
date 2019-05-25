using System;

// Token: 0x02000193 RID: 403
public class BeatmapData
{
	// Token: 0x170000EF RID: 239
	// (get) Token: 0x0600067E RID: 1662 RVA: 0x00018451 File Offset: 0x00016651
	// (set) Token: 0x0600067F RID: 1663 RVA: 0x00018459 File Offset: 0x00016659
	public BeatmapLineData[] beatmapLinesData { get; private set; }

	// Token: 0x170000F0 RID: 240
	// (get) Token: 0x06000680 RID: 1664 RVA: 0x00018462 File Offset: 0x00016662
	// (set) Token: 0x06000681 RID: 1665 RVA: 0x0001846A File Offset: 0x0001666A
	public BeatmapEventData[] beatmapEventData { get; private set; }

	// Token: 0x170000F1 RID: 241
	// (get) Token: 0x06000682 RID: 1666 RVA: 0x00018473 File Offset: 0x00016673
	// (set) Token: 0x06000683 RID: 1667 RVA: 0x0001847B File Offset: 0x0001667B
	public int notesCount { get; private set; }

	// Token: 0x170000F2 RID: 242
	// (get) Token: 0x06000684 RID: 1668 RVA: 0x00018484 File Offset: 0x00016684
	// (set) Token: 0x06000685 RID: 1669 RVA: 0x0001848C File Offset: 0x0001668C
	public int obstaclesCount { get; private set; }

	// Token: 0x170000F3 RID: 243
	// (get) Token: 0x06000686 RID: 1670 RVA: 0x00018495 File Offset: 0x00016695
	// (set) Token: 0x06000687 RID: 1671 RVA: 0x0001849D File Offset: 0x0001669D
	public int bombsCount { get; private set; }

	// Token: 0x06000688 RID: 1672 RVA: 0x000184A8 File Offset: 0x000166A8
	public BeatmapData(BeatmapLineData[] beatmapLinesData, BeatmapEventData[] beatmapEventData)
	{
		this.beatmapLinesData = beatmapLinesData;
		this.beatmapEventData = beatmapEventData;
		for (int i = 0; i < beatmapLinesData.Length; i++)
		{
			foreach (BeatmapObjectData beatmapObjectData in beatmapLinesData[i].beatmapObjectsData)
			{
				if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Note)
				{
					NoteType noteType = ((NoteData)beatmapObjectData).noteType;
					if (noteType == NoteType.NoteA || noteType == NoteType.NoteB)
					{
						int num = this.notesCount;
						this.notesCount = num + 1;
					}
					else if (noteType == NoteType.Bomb)
					{
						int num = this.bombsCount;
						this.bombsCount = num + 1;
					}
				}
				else if (beatmapObjectData.beatmapObjectType == BeatmapObjectType.Obstacle)
				{
					int num = this.obstaclesCount;
					this.obstaclesCount = num + 1;
				}
			}
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x00018564 File Offset: 0x00016764
	public virtual BeatmapData GetCopy()
	{
		BeatmapLineData[] array = new BeatmapLineData[this.beatmapLinesData.Length];
		for (int i = 0; i < this.beatmapLinesData.Length; i++)
		{
			BeatmapLineData beatmapLineData = this.beatmapLinesData[i];
			BeatmapLineData beatmapLineData2 = new BeatmapLineData();
			beatmapLineData2.beatmapObjectsData = new BeatmapObjectData[beatmapLineData.beatmapObjectsData.Length];
			for (int j = 0; j < beatmapLineData.beatmapObjectsData.Length; j++)
			{
				BeatmapObjectData beatmapObjectData = beatmapLineData.beatmapObjectsData[j];
				beatmapLineData2.beatmapObjectsData[j] = beatmapObjectData.GetCopy();
			}
			array[i] = beatmapLineData2;
		}
		BeatmapEventData[] array2 = new BeatmapEventData[this.beatmapEventData.Length];
		for (int k = 0; k < this.beatmapEventData.Length; k++)
		{
			BeatmapEventData beatmapEventData = this.beatmapEventData[k];
			array2[k] = beatmapEventData.GetCopy();
		}
		return new BeatmapData(array, array2);
	}
}
