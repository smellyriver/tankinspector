using Smellyriver.Serialization;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Smellyriver.Utilities;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    public class BattleTierRules
    {
        private const string BattleTierRulesConfigFile = "Additional/BattleTiers.xml";

        public static BattleTierRules Current { get; private set; }

        static BattleTierRules()
        {
            BattleTierRules.Current = BattleTierRules.Load(BattleTierRulesConfigFile);
        }

        public static BattleTierRules Load(string file)
        {
            using (Diagnostics.PotentialExceptionRegion)
            {
                return Serializer.Deserialize<BattleTierRules>(file);
            }
        }

        [XmlAttribute]
        public string GameVersion { get; set; }

        [XmlArrayItem("Rule")]
        public List<RegularBattleTierRule> Regular { get; set; }
        [XmlArrayItem("Rule")]
        public List<SpecialBattleTierRule> Special { get; set; }

        private Dictionary<int, Dictionary<TankClass, BattleTierSpan>> _regularRules;
        private Dictionary<string, BattleTierSpan> _specialRules;

        private BattleTierRules()
        {
            this.Regular = new List<RegularBattleTierRule>();
            this.Special = new List<SpecialBattleTierRule>();
        }

        public void Save(string file)
        {
            Serializer.Serialize(this, file);
        }

        internal void AnalyseRules()
        {
            _regularRules = new Dictionary<int, Dictionary<TankClass, BattleTierSpan>>();

            foreach (var regularRule in this.Regular)
            {
				_regularRules.GetOrCreate(regularRule.VehicleTier, out Dictionary<TankClass, BattleTierSpan> classDict);
				classDict[regularRule.Class] = regularRule.BattleTier;
            }

            _specialRules = new Dictionary<string, BattleTierSpan>();

            foreach (var specialRule in this.Special)
            {
                _specialRules[specialRule.Vehicle] = specialRule.BattleTier;
            }
        }

        internal BattleTierSpan GetBattleTiers(Tank tank)
        {
            if (_regularRules == null)
                this.AnalyseRules();

			if (_specialRules.TryGetValue(tank.ColonFullKey, out BattleTierSpan battleTier))
				return battleTier;
	        if (_regularRules.ContainsKey(tank.Tier) && _regularRules[tank.Tier].ContainsKey(tank.Class))
		        return _regularRules[tank.Tier][tank.Class];
	        return new BattleTierSpan(tank.Tier, Math.Min(11, tank.Tier + 2));
        }

        internal void WriteBattleTiers(IEnumerable<Tank> tanks)
        {

            foreach (var tank in tanks)
            {
				if (_specialRules.TryGetValue(tank.ColonFullKey, out BattleTierSpan battleTier))
					tank.BattleTiers = battleTier;
				else
					tank.BattleTiers = _regularRules[tank.Tier][tank.Class];
			}
        }
    }
}
