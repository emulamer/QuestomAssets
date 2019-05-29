using System;

public abstract class BeatmapObjectData
{
    public BeatmapObjectType beatmapObjectType { get; private set; }
	public float time { get; private set; }
	public int lineIndex { get; protected set; }
    public int id { get; private set; }
}
