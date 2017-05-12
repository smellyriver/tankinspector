using System;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class Hull : DamageableModule , IHasArmor, IHull
    {
        public override ModuleType Type => ModuleType.Hull;

	    protected override bool IsWrapped => false;

	    private AmmoBay _ammoBay;
        [StatSubItem("AmmoBay")]
        public AmmoBay AmmoBay
        {
            get => _ammoBay;
	        set => _ammoBay = value;
        }

        IAmmoBay IHull.AmmoBay => _ammoBay;

	    private Armor _armor;
        public Armor Armor
        {
            get => _armor;
	        set => _armor = value;
        }

        private Vector3D _turretPosition;
        public Vector3D TurretPosition
        {
            get => _turretPosition;
	        set => _turretPosition = value;
        }

        private PrimaryArmorKeys _primaryArmorKeys;

        [Stat("HullFrontalArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double FrontalArmor => _armor.ArmorGroups[_primaryArmorKeys.Front].Value;

	    [Stat("HullSideArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double SideArmor => _armor.ArmorGroups[_primaryArmorKeys.Side].Value;

	    [Stat("HullRearArmorFullName", DataAnalysis.ComparisonMode.HigherBetter)]
        public double RearArmor => _armor.ArmorGroups[_primaryArmorKeys.Rear].Value;

	    public Hull(Database database)
            : base(database)
        {
                
        }


        protected override bool DeserializeSection(string name, XmlReader reader)
        {
            switch (name)
            {
                case "armor":
                    _armor = new Armor(((Tank)this.Owner).Nation.Database);
                    _armor.Deserialize(reader);
                    return true;

                case "primaryArmor":
                    reader.Read(out _primaryArmorKeys);
                    return true;

                case "ammoBayHealth":
                    _ammoBay = new AmmoBay(this.Database);
                    _ammoBay.Deserialize(reader);
                    return true;

                case "turretPositions":

                    // turret position of usa T20 appeared twice, maozi sucks
                    if (this.TurretPosition != null)
                        return false;

                    reader.ReadStartElement("turret");
                    reader.Read(out _turretPosition);
                    reader.ReadEndElement();
                    return true;

                case "swinging":
                case "exhaust":
                case "fakeTurrets":
                case "emblemSlots":
                case "camouflage":
                case "animateEmblemSlots":
                case "AODecals": //?
                case "turretHardPoints":
                case "variants": // todo
                case "hangarShadowTexture":

                    return false;


                default:
                    if (base.DeserializeSection(name, reader))
                        return true;
                    else
                        return false;
            }

        }
    }
}
