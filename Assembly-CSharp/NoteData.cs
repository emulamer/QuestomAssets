using System;


public class NoteData : BeatmapObjectData
{

	public NoteType noteType { get; private set; }

	public NoteCutDirection cutDirection { get; private set; }

    public NoteLineLayer noteLineLayer { get; private set; }

	public NoteLineLayer startNoteLineLayer { get; private set; }


	public int flipLineIndex { get; private set; }

	public float flipYSide { get; private set; }

	public float timeToNextBasicNote { get; set; }

	public float timeToPrevBasicNote { get; private set; }

    
}
