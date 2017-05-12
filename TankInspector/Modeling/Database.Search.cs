using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Smellyriver.TankInspector.Modeling
{
	internal partial class Database
    {
	    private static string GetPinyinString(string str)
        {
            var builder = new StringBuilder();
            foreach (char c in str)
            {
                var pinyinFirstChar = Database.GetPinyinFirstChar(c.ToString());

                if (pinyinFirstChar == null)
                    builder.Append(c);
                else
                    builder.Append(pinyinFirstChar);
            }
            return builder.ToString();
        }

	    private static string GetPinyinFirstChar(string c)
        {
            var array =  Encoding.Default.GetBytes(c);
            if (array.Length != 2)
                return null;

            int i = (short)(array[0] - '\0') * 256 + ((short)(array[1] - '\0'));
            if (i < 0xB0A1) return null;
            if (i < 0xB0C5) return "a";
            if (i < 0xB2C1) return "b";
            if (i < 0xB4EE) return "c";
            if (i < 0xB6EA) return "d";
            if (i < 0xB7A2) return "e";
            if (i < 0xB8C1) return "f";
            if (i < 0xB9FE) return "g";
            if (i < 0xBBF7) return "h";
            if (i < 0xBFA6) return "j";
            if (i < 0xC0AC) return "k";
            if (i < 0xC2E8) return "l";
            if (i < 0xC4C3) return "m";
            if (i < 0xC5B6) return "n";
            if (i < 0xC5BE) return "o";
            if (i < 0xC6DA) return "p";
            if (i < 0xC8BB) return "q";
            if (i < 0xC8F6) return "r";
            if (i < 0xCBFA) return "s";
            if (i < 0xCDDA) return "t";
            if (i < 0xCEF4) return "w";
            if (i < 0xD1B9) return "x";
            if (i < 0xD4D1) return "y";
            if (i < 0xD7FA) return "z";
            return null;
        }


        public IEnumerable<ITankInfo> Search(string keyword, int tier = -1, TankClass tankClass = TankClass.Mixed, bool includeVirtualTanks = false)
        {
            if (keyword.Length == 0 && tier == -1 && tankClass == TankClass.Mixed)
                return new ITankInfo[0];

            List<ITankInfo> result = new List<ITankInfo>();

            if (includeVirtualTanks)
                result.AddRange(
                    this.BenchmarkTanks.Values
                    .SelectMany(g => g.Values)
                    .Where(t => this.MatchTank(t, keyword, tier, tankClass)));

            foreach (var nation in this.Nations.Values)
            {
                result.AddRange(
                    nation.TankInfoCollection
                    .Where(t => this.MatchTank(t, keyword, tier, tankClass)));
            }

            return result;
        }

        private bool MatchTank(ITankInfo tank, string keyword, int tier = -1, TankClass tankClass = TankClass.Mixed)
        {
            return this.MatchTier(tank.Tier, tier)
                && this.MatchClass(tank.Class, tankClass)
                && this.MatchKeyword(tank, keyword);
        }

        private bool MatchClass(TankClass tankClass, TankClass key)
        {
	        if (key == TankClass.Mixed)
                return true;
	        return key == tankClass;
        }

        private bool MatchTier(int tier, int key)
        {
	        if (key == -1)
                return true;
	        return tier == key;
        }

        private bool MatchKeyword(ITankInfo tank, string keyword)
        {
            return this.MatchKeyword(tank.Name, keyword)
                || this.MatchKeyword(tank.ShortName, keyword)
                || this.MatchKeyword(Database.GetPinyinString(tank.Name), keyword);
        }

        private bool MatchKeyword(string name, string keyword)
        {
            if (keyword.Length == 0)
                return true;

            name = name.ToLowerInvariant();
            keyword = keyword.ToLowerInvariant();
            if (name.Contains(keyword))
                return true;

            name = new string(name.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
            return name.Contains(keyword);
        }

    }
}
