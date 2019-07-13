using System;
using System.Collections.Generic;
using System.Text;

namespace QuestomAssets.AssetsChanger
{
    public class PropertySheet
    {
        public PropertySheet(AssetsFile file, AssetsObject owner, AssetsReader reader)
        {
            Parse(file, owner, reader);
        }

        public PropertySheet()
        {

        }

        public void Parse(AssetsFile file, AssetsObject owner, AssetsReader reader)
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count;i++)
            {
                TexEnvs.Add(new Map<string, TexEnv>(reader.ReadString(), new TexEnv(file, owner, reader)));
            }
            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Floats.Add(new Map<string, Single>(reader.ReadString(), reader.ReadSingle()));
            }

            count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                Colors.Add(new Map<string, Color>(reader.ReadString(), new Color(reader)));
            }
        }

        public void Write(AssetsWriter writer)
        {
            writer.WriteArrayOf(TexEnvs, (x, w) =>
            {
                w.Write(x.First);
                x.Second.Write(w);
            });
            writer.WriteArrayOf(Floats, (x, w) =>
            {
                w.Write(x.First);
                w.Write(x.Second);
            });
            writer.WriteArrayOf(Colors, (x, w) =>
            {
                w.Write(x.First);
                x.Second.Write(w);
            });
        }

        public List<Map<string, TexEnv>> TexEnvs { get; set; } = new List<Map<string, TexEnv>>();
        public List<Map<string, Single>> Floats { get; set; } = new List<Map<string, float>>();
        public List<Map<string, Color>> Colors { get; set; } = new List<Map<string, Color>>();

    }
}
