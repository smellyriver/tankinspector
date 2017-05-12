using System;
using System.Runtime.Serialization;

namespace Smellyriver.TankInspector.Modeling
{
    [Serializable]
    internal abstract class DatabaseObject : IDatabaseObject
    {
        [NonSerialized]
        private Database _database;
        public Database Database => _database;

	    private readonly DatabaseKey _databaseKey;

        public DatabaseObject(Database database)
        {
            _database = database;
            _databaseKey = _database.Key;
        }


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
            _database = Database.GetDatabase(_databaseKey);
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
