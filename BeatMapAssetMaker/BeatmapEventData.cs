using System;

// Token: 0x02000195 RID: 405
public class BeatmapEventData
{
	// Token: 0x170000F5 RID: 245
	// (get) Token: 0x06000690 RID: 1680 RVA: 0x000186A3 File Offset: 0x000168A3
	// (set) Token: 0x06000691 RID: 1681 RVA: 0x000186AB File Offset: 0x000168AB
	public BeatmapEventType type { get; protected set; }

	// Token: 0x170000F6 RID: 246
	// (get) Token: 0x06000692 RID: 1682 RVA: 0x000186B4 File Offset: 0x000168B4
	// (set) Token: 0x06000693 RID: 1683 RVA: 0x000186BC File Offset: 0x000168BC
	public float time { get; protected set; }

	// Token: 0x170000F7 RID: 247
	// (get) Token: 0x06000694 RID: 1684 RVA: 0x000186C5 File Offset: 0x000168C5
	// (set) Token: 0x06000695 RID: 1685 RVA: 0x000186CD File Offset: 0x000168CD
	public int value { get; protected set; }

	// Token: 0x06000696 RID: 1686 RVA: 0x000186D6 File Offset: 0x000168D6
	public BeatmapEventData(float time, BeatmapEventType type, int value)
	{
		this.time = time;
		this.type = type;
		this.value = value;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000186F3 File Offset: 0x000168F3
	public virtual BeatmapEventData GetCopy()
	{
		return new BeatmapEventData(this.time, this.type, this.value);
	}
}
