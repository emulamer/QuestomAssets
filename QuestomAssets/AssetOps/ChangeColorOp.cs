using QuestomAssets.BeatSaber;
using QuestomAssets.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class ChangeColorOp: AssetOp
    {
        public override bool IsWriteOp => true;

        public ColorType _colorType { get; private set; }
        public BeatSaberColor _color { get; private set; }
        public ChangeColorOp(ColorType colorType, BeatSaberColor color)
        {
            _colorType = colorType;
            _color = color;
        }

        public string SongID { get; private set; }
        internal override void PerformOp(OpContext context)
        {
            var colorManager = context.Manager.MassFirstOrDefaultAsset<ColorManager>(x => true, false)?.Object;
            if (colorManager == null)
            {
                Log.LogErr($"ChangeColorOp could not get the ColorManager class from assets!");
                throw new Exception("Unable to find the color manager asset!");
            }
            SimpleColorSO color = null;
            switch (_colorType)
            {
                case ColorType.LeftColor:
                    color = colorManager.ColorA.Object;
                    break;
                case ColorType.RightColor:
                    color = colorManager.ColorB.Object;
                    break;
            }

            if (color == null)
            {
                Log.LogErr($"Color type {_colorType} not found!");
                throw new Exception($"Color type {_colorType} not found!");
            }

            color.Color.A = (float)_color.A / (float)255;
            color.Color.R = (float)_color.R / (float)255;
            color.Color.G = (float)_color.G / (float)255;
            color.Color.B = (float)_color.B / (float)255;
            color.ObjectInfo.ParentFile.HasChanges = true;
        }
    }
}
