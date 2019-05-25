using System;

// Token: 0x020001A6 RID: 422
public class ObstacleData : BeatmapObjectData
{
	// Token: 0x17000119 RID: 281
	// (get) Token: 0x060006E6 RID: 1766 RVA: 0x00018E5B File Offset: 0x0001705B
	// (set) Token: 0x060006E7 RID: 1767 RVA: 0x00018E63 File Offset: 0x00017063
	public ObstacleType obstacleType { get; private set; }

	// Token: 0x1700011A RID: 282
	// (get) Token: 0x060006E8 RID: 1768 RVA: 0x00018E6C File Offset: 0x0001706C
	// (set) Token: 0x060006E9 RID: 1769 RVA: 0x00018E74 File Offset: 0x00017074
	public float duration { get; private set; }

	// Token: 0x1700011B RID: 283
	// (get) Token: 0x060006EA RID: 1770 RVA: 0x00018E7D File Offset: 0x0001707D
	// (set) Token: 0x060006EB RID: 1771 RVA: 0x00018E85 File Offset: 0x00017085
	public int width { get; private set; }

	// Token: 0x060006EC RID: 1772 RVA: 0x00018E8E File Offset: 0x0001708E
	public ObstacleData(int id, float time, int lineIndex, ObstacleType obstacleType, float duration, int width) : base(BeatmapObjectType.Obstacle, id, time, lineIndex)
	{
		this.obstacleType = obstacleType;
		this.duration = duration;
		this.width = width;
	}

	// Token: 0x060006ED RID: 1773 RVA: 0x00018EB2 File Offset: 0x000170B2
	public virtual void UpdateDuration(float duration)
	{
		this.duration = duration;
	}

	// Token: 0x060006EE RID: 1774 RVA: 0x00018EBB File Offset: 0x000170BB
	public override BeatmapObjectData GetCopy()
	{
		return new ObstacleData(base.id, base.time, base.lineIndex, this.obstacleType, this.duration, this.width);
	}

	// Token: 0x060006EF RID: 1775 RVA: 0x00018EE6 File Offset: 0x000170E6
	public override void MirrorLineIndex(int lineCount)
	{
		base.lineIndex = lineCount - this.width - base.lineIndex;
	}
}
