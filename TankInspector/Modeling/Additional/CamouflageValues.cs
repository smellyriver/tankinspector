using Smellyriver.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    public class CamouflageValues
    {
        private const string CamoflagueValuesConfigFile = "Additional/Camouflage.xml";

        public static CamouflageValues Current { get; }

        static CamouflageValues()
        {
            CamouflageValues.Current = CamouflageValues.Load(CamoflagueValuesConfigFile);

            //HashSet<string> keys = new HashSet<string>();
            //foreach (var entry in Current.Entries)
            //{
            //    if (keys.Contains(entry.Vehicle))
            //        System.Diagnostics.Trace.WriteLine(entry.Vehicle);
            //    else
            //        keys.Add(entry.Vehicle);


            //    var tank = Database.Current.Tanks.Where(t => t.Name == entry.Vehicle).FirstOrDefault();
            //    if (tank != null)
            //        entry.Vehicle = tank.ColonFullKey;
            //    else
            //        System.Diagnostics.Trace.WriteLine(entry.Vehicle);
            //}

            //Serializer.Serialize(Current, CamoflagueValuesConfigFile);
        }

        public static CamouflageValues Load(string file)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                return Serializer.Deserialize<CamouflageValues>(file);
            }
        }


        [XmlAttribute]
        public string GameVersion { get; set; }

        [XmlArrayItem("Entry")]
        public List<CamouflageValueEntry> Entries { get; set; }

        private readonly Lazy<Dictionary<string, CamouflageValue>> _entryDict;

        public CamouflageValues()
        {
            this.Entries = new List<CamouflageValueEntry>();
            _entryDict = new Lazy<Dictionary<string, CamouflageValue>>(AnalyseValues);
        }

        private Dictionary<string, CamouflageValue> AnalyseValues()
        {
            return this.Entries.ToDictionary(e => e.Vehicle, e => e.Value);
        }

        internal CamouflageValue GetCamouflageValue(Tank tank)
        {
			if (_entryDict.Value.TryGetValue(tank.ColonFullKey, out CamouflageValue value))
				return value;

			return null;
        }

    }
}
