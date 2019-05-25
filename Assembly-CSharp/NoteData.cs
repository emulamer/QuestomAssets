using System;


// Token: 0x020001A2 RID: 418
public class NoteData : BeatmapObjectData
{
	// Token: 0x17000111 RID: 273
	// (get) Token: 0x060006CC RID: 1740 RVA: 0x00018B46 File Offset: 0x00016D46
	// (set) Token: 0x060006CD RID: 1741 RVA: 0x00018B4E File Offset: 0x00016D4E
	public NoteType noteType { get; private set; }

	// Token: 0x17000112 RID: 274
	// (get) Token: 0x060006CE RID: 1742 RVA: 0x00018B57 File Offset: 0x00016D57
	// (set) Token: 0x060006CF RID: 1743 RVA: 0x00018B5F File Offset: 0x00016D5F
	public NoteCutDirection cutDirection { get; private set; }

	// Token: 0x17000113 RID: 275
	// (get) Token: 0x060006D0 RID: 1744 RVA: 0x00018B68 File Offset: 0x00016D68
	// (set) Token: 0x060006D1 RID: 1745 RVA: 0x00018B70 File Offset: 0x00016D70
	public NoteLineLayer noteLineLayer { get; private set; }

	// Token: 0x17000114 RID: 276
	// (get) Token: 0x060006D2 RID: 1746 RVA: 0x00018B79 File Offset: 0x00016D79
	// (set) Token: 0x060006D3 RID: 1747 RVA: 0x00018B81 File Offset: 0x00016D81
	public NoteLineLayer startNoteLineLayer { get; private set; }

	// Token: 0x17000115 RID: 277
	// (get) Token: 0x060006D4 RID: 1748 RVA: 0x00018B8A File Offset: 0x00016D8A
	// (set) Token: 0x060006D5 RID: 1749 RVA: 0x00018B92 File Offset: 0x00016D92
	public int flipLineIndex { get; private set; }

	// Token: 0x17000116 RID: 278
	// (get) Token: 0x060006D6 RID: 1750 RVA: 0x00018B9B File Offset: 0x00016D9B
	// (set) Token: 0x060006D7 RID: 1751 RVA: 0x00018BA3 File Offset: 0x00016DA3
	public float flipYSide { get; private set; }

	// Token: 0x17000117 RID: 279
	// (get) Token: 0x060006D8 RID: 1752 RVA: 0x00018BAC File Offset: 0x00016DAC
	// (set) Token: 0x060006D9 RID: 1753 RVA: 0x00018BB4 File Offset: 0x00016DB4
	public float timeToNextBasicNote { get; set; }

	// Token: 0x17000118 RID: 280
	// (get) Token: 0x060006DA RID: 1754 RVA: 0x00018BBD File Offset: 0x00016DBD
	// (set) Token: 0x060006DB RID: 1755 RVA: 0x00018BC5 File Offset: 0x00016DC5
	public float timeToPrevBasicNote { get; private set; }

	// Token: 0x060006DC RID: 1756 RVA: 0x00018BD0 File Offset: 0x00016DD0
	public override BeatmapObjectData GetCopy()
	{
		return new NoteData(base.id, base.time, base.lineIndex, this.noteLineLayer, this.startNoteLineLayer, this.noteType, this.cutDirection, this.timeToNextBasicNote, this.timeToPrevBasicNote, this.flipLineIndex, this.flipYSide);
	}

	// Token: 0x060006DD RID: 1757 RVA: 0x00018C24 File Offset: 0x00016E24
	public NoteData(int id, float time, int lineIndex, NoteLineLayer noteLineLayer, NoteLineLayer startNoteLineLayer, NoteType noteType, NoteCutDirection cutDirection, float timeToNextBasicNote, float timeToPrevBasicNote) : base(BeatmapObjectType.Note, id, time, lineIndex)
	{
		this.noteLineLayer = noteLineLayer;
		this.startNoteLineLayer = startNoteLineLayer;
		this.noteType = noteType;
		this.cutDirection = cutDirection;
		this.flipLineIndex = lineIndex;
		this.flipYSide = 0f;
		this.timeToNextBasicNote = timeToNextBasicNote;
		this.timeToPrevBasicNote = timeToPrevBasicNote;
	}

	// Token: 0x060006DE RID: 1758 RVA: 0x00018C80 File Offset: 0x00016E80
	public NoteData(int id, float time, int lineIndex, NoteLineLayer noteLineLayer, NoteLineLayer startNoteLineLayer, NoteType noteType, NoteCutDirection cutDirection, float timeToNextBasicNote, float timeToPrevBasicNote, int flipLineIndex, float flipYSide) : this(id, time, lineIndex, noteLineLayer, startNoteLineLayer, noteType, cutDirection, timeToNextBasicNote, timeToPrevBasicNote)
	{
		this.flipLineIndex = flipLineIndex;
		this.flipYSide = flipYSide;
	}

	// Token: 0x060006DF RID: 1759 RVA: 0x00018CB4 File Offset: 0x00016EB4
	public virtual void SetNoteFlipToNote(NoteData targetNote)
	{
		this.flipLineIndex = targetNote.lineIndex;
		this.flipYSide = (float)((base.lineIndex > targetNote.lineIndex) ? 1 : -1);
		if ((base.lineIndex > targetNote.lineIndex && this.noteLineLayer < targetNote.noteLineLayer) || (base.lineIndex < targetNote.lineIndex && this.noteLineLayer > targetNote.noteLineLayer))
		{
			this.flipYSide *= -1f;
		}
	}

	// Token: 0x060006E0 RID: 1760 RVA: 0x00018D30 File Offset: 0x00016F30
	public virtual void SwitchNoteType()
	{
		if (this.noteType == NoteType.NoteA)
		{
			this.noteType = NoteType.NoteB;
			return;
		}
		if (this.noteType == NoteType.NoteB)
		{
			this.noteType = NoteType.NoteA;
		}
	}

	// Token: 0x060006E1 RID: 1761 RVA: 0x00018D54 File Offset: 0x00016F54
	public virtual void MirrorTransformCutDirection()
	{
		if (this.cutDirection == NoteCutDirection.Left)
		{
			this.cutDirection = NoteCutDirection.Right;
			return;
		}
		if (this.cutDirection == NoteCutDirection.Right)
		{
			this.cutDirection = NoteCutDirection.Left;
			return;
		}
		if (this.cutDirection == NoteCutDirection.UpLeft)
		{
			this.cutDirection = NoteCutDirection.UpRight;
			return;
		}
		if (this.cutDirection == NoteCutDirection.UpRight)
		{
			this.cutDirection = NoteCutDirection.UpLeft;
			return;
		}
		if (this.cutDirection == NoteCutDirection.DownLeft)
		{
			this.cutDirection = NoteCutDirection.DownRight;
			return;
		}
		if (this.cutDirection == NoteCutDirection.DownRight)
		{
			this.cutDirection = NoteCutDirection.DownLeft;
		}
	}

	// Token: 0x060006E2 RID: 1762 RVA: 0x00018DC6 File Offset: 0x00016FC6
	public virtual void SetNoteToAnyCutDirection()
	{
		this.cutDirection = NoteCutDirection.Any;
	}

	// Token: 0x060006E3 RID: 1763 RVA: 0x00018DD0 File Offset: 0x00016FD0
	public virtual void TransformNoteAOrBToRandomType()
	{
		if (this.noteType != NoteType.NoteA && this.noteType != NoteType.NoteB)
		{
			return;
		}
        Random r = new Random();
		if (r.NextDouble() > 0.6f)
		{
			this.noteType = ((this.noteType == NoteType.NoteA) ? NoteType.NoteB : NoteType.NoteA);
		}
		this.flipLineIndex = base.lineIndex;
		this.flipYSide = 0f;
	}

	// Token: 0x060006E4 RID: 1764 RVA: 0x00018E2E File Offset: 0x0001702E
	public override void MirrorLineIndex(int lineCount)
	{
		base.lineIndex = lineCount - 1 - base.lineIndex;
		this.flipLineIndex = lineCount - 1 - this.flipLineIndex;
	}
}
