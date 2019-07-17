using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetOps
{
    public class CreateAssetOp : AssetOp
    {
        public override bool IsWriteOp => true;

        private byte[] _replaceDataWith;
        private bool _allowOverwriteName;
        public CreateAssetOp(byte[] replaceDataWith, bool allowOverwriteName)
        {
            _replaceDataWith = replaceDataWith;
            _allowOverwriteName = allowOverwriteName;
        }

        internal override void PerformOp(OpContext context)
        {
            var asset = _locator.Locate(context.Manager, false);
            string oldName = null;
            if (asset as IHaveName != null && !_allowOverwriteName)
            {
                oldName = (asset as IHaveName).Name;
            }
            using (var ms = new MemoryStream(_replaceDataWith))
            {
                using (AssetsReader reader = new AssetsReader(ms))
                {
                    asset.ObjectInfo.DataOffset = 0;
                    asset.ObjectInfo.DataSize = _replaceDataWith.Length;
                    asset.Parse(reader);

                    asset.ObjectInfo.DataOffset = -1;
                    asset.ObjectInfo.DataSize = -1;
                    if (oldName != null)
                        (asset as IHaveName).Name = oldName;
                }
            }
        }
    }
}
