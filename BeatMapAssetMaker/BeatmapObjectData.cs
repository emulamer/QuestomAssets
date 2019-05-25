using System;

// Token: 0x02000198 RID: 408
public abstract class BeatmapObjectData
{
	// Token: 0x170000F8 RID: 248
	// (get) Token: 0x06000699 RID: 1689 RVA: 0x0001870C File Offset: 0x0001690C
	// (set) Token: 0x0600069A RID: 1690 RVA: 0x00018714 File Offset: 0x00016914
	public BeatmapObjectType beatmapObjectType { get; private set; }

	// Token: 0x170000F9 RID: 249
	// (get) Token: 0x0600069B RID: 1691 RVA: 0x0001871D File Offset: 0x0001691D
	// (set) Token: 0x0600069C RID: 1692 RVA: 0x00018725 File Offset: 0x00016925
	public float time { get; private set; }

	// Token: 0x170000FA RID: 250
	// (get) Token: 0x0600069D RID: 1693 RVA: 0x0001872E File Offset: 0x0001692E
	// (set) Token: 0x0600069E RID: 1694 RVA: 0x00018736 File Offset: 0x00016936
	public int lineIndex { get; protected set; }

	// Token: 0x170000FB RID: 251
	// (get) Token: 0x0600069F RID: 1695 RVA: 0x0001873F File Offset: 0x0001693F
	// (set) Token: 0x060006A0 RID: 1696 RVA: 0x00018747 File Offset: 0x00016947
	public int id { get; private set; }

	// Token: 0x060006A1 RID: 1697 RVA: 0x00018750 File Offset: 0x00016950
	public BeatmapObjectData(BeatmapObjectType beatmapObjectType, int id, float time, int lineIndex)
	{
		this.beatmapObjectType = beatmapObjectType;
		this.id = id;
		this.time = time;
		this.lineIndex = lineIndex;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00018775 File Offset: 0x00016975
	public virtual void MirrorLineIndex(int lineCount)
	{
		this.lineIndex = lineCount - 1 - this.lineIndex;
	}

	// Token: 0x060006A3 RID: 1699
	public abstract BeatmapObjectData GetCopy();
}
