using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BeatmapLevelCollectionSO
{
    public BeatmapLevelCollectionSO()
    {
        _beatmapLevels = new List<UPtr>();
    }
    public List<UPtr> _beatmapLevels;

    public void Write(AlignedStream s)
    {
        s.Write(_beatmapLevels.Count);
        foreach (var b in _beatmapLevels)
        {
            s.Write(b);
        }
    }
}

