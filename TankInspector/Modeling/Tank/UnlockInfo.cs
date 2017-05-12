using System;
using System.Runtime.Serialization;
using System.Xml;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal class UnlockInfo : ISectionDeserializable
    {
        public int ExperiencePrice { get; private set; }

        [NonSerialized]
        private TankObject _tankObject;
        public TankObject TankObject => _tankObject;

	    private string _tankObjectType;
        private string _key;

        [NonSerialized]
        private bool _isResolved;

        public bool IsResolved => _isResolved;

	    public void Deserialize(XmlReader reader)
        {
            SectionDeserializableImpl.Deserialize(this, reader);
        }

        public void Resolve(Tank tank)
        {
            if (!_isResolved)
            {
                switch (_tankObjectType)
                {
                    case "vehicle":
                        _tankObject = tank.GetTankObject<Tank>(this._key);
                        break;
                    case "chassis":
                        _tankObject = tank.GetTankObject<Chassis>(this._key);
                        break;
                    case "gun":
                        _tankObject = tank.GetTankObject<Gun>(this._key);
                        break;
                    case "engine":
                        _tankObject = tank.GetTankObject<Engine>(this._key);
                        break;
                    case "radio":
                        _tankObject = tank.GetTankObject<Radio>(this._key);
                        break;
                    case "turret":
                        _tankObject = tank.GetTankObject<Turret>(this._key);
                        break;
                    default:
                        throw new NotSupportedException();
                }

                _isResolved = true;
            }
        }

        bool ISectionDeserializable.DeserializeSection(string name, XmlReader reader)
        {
            if (name == SectionDeserializableImpl.TitleToken)
            {
                _tankObjectType = reader.Name;
                return true;
            }

            _key = reader.ReadString().Trim();

            reader.ReadStartElement("cost");
            var experience = reader.ReadContentAsDouble();
            this.ExperiencePrice = (int)experience;
            reader.ReadEndElement();

            _isResolved = false;
            return true;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            _isResolved = false;
        }

        bool ISectionDeserializable.IsWrapped => true;
    }
}
