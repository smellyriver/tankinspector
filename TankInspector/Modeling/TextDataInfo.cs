using System;
using System.Linq;

namespace Smellyriver.TankInspector.Modeling
{
	internal class TextDataInfo
    {
        public string BaseName { get; set; }
        public string MessageId { get; set; }


        public static TextDataInfo Parse(string info)
        {
            if (info.First() != '#')
            {
                return new TextDataInfo { BaseName = "", MessageId = info };
            }

            //if (info.Last() != '.')
            //{
            //    throw new InvalidOperationException(@"Text Data Info must end with '.' !");
            //}

            var rawInfo = info.Substring(1, info.Length - 1);

            var infos = rawInfo.Split(':');

            if (infos.Length != 2)
            {
                throw new InvalidOperationException(@"Text Data Info must Split with ',' ");
            }

            var textDataInfo = new TextDataInfo();
            textDataInfo.BaseName = infos[0];
            textDataInfo.MessageId = infos[1];

            return textDataInfo;
        }

    }
}
