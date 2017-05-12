using System;
using System.Text;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal struct TankObjectKey
    {

        public static TankObjectKey Create(TankObjectKey? ownerKey, TankObjectType objectType, string key)
        {
            StringBuilder keyBuilder = new StringBuilder();
            if (ownerKey != null)
            {
                keyBuilder.Append('[')
                    .Append(ownerKey.ToString())
                    .Append(']');
            }

            keyBuilder.Append(key);

            return new TankObjectKey(objectType, keyBuilder.ToString());
        }

        public TankObjectType Type;
        public string Key;

        public TankObjectKey(TankObjectType type, string key)
            : this()
        {
            this.Type = type;
            this.Key = key;
        }

        public override int GetHashCode()
        {
            return (this.Type.GetHashCode() << 24) + this.Key.GetHashCode();
        }

        public override string ToString()
        {
            return $"{this.Type}:{this.Key}";
        }

        public override bool Equals(object obj)
        {
	        if (obj == null)
                return false;
	        if (obj is TankObjectKey)
	        {
		        var key = (TankObjectKey)obj;
		        return key.Key == this.Key && key.Type == this.Type;
	        }
	        return false;
        }
    }
}
