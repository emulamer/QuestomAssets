using System;

// Token: 0x020001A5 RID: 421
public static class NoteTypeExtensions
{
	// Token: 0x060006E5 RID: 1765 RVA: 0x00018E50 File Offset: 0x00017050
	public static bool IsBasicNote(this NoteType noteType)
	{
		return noteType == NoteType.NoteA || noteType == NoteType.NoteB;
	}
}
