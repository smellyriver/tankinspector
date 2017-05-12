using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    [DebuggerDisplay("{Key}")]
    internal class VirtualCommodity : ICommodity
    {
       
        public CurrencyType CurrencyType { get; internal set; }

        public string Key { get; internal set; }

        public bool NotInShop { get; internal set; }

        public int Price { get; internal set; }


        [OnSerializing]
        private void PrivateOnSerializing(StreamingContext context)
        {
            this.OnSerializing(context);
        }

        [OnSerialized]
        private void PrivateOnSerialized(StreamingContext context)
        {
            this.OnSerialized(context);
        }

        [OnDeserializing]
        private void PrivateOnDeserializing(StreamingContext context)
        {
            this.OnDeserializing(context);
        }

        [OnDeserialized]
        private void PrivateOnDeserialized(StreamingContext context)
        {
            this.OnDeserialized(context);
        }

        protected virtual void OnSerializing(StreamingContext context)
        {

        }

        protected virtual void OnSerialized(StreamingContext context)
        {

        }

        protected virtual void OnDeserializing(StreamingContext context)
        {

        }

        protected virtual void OnDeserialized(StreamingContext context)
        {

        }
    }
}
