using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GridFS;
using MongoDB.Driver.Linq;

namespace Persistence
{
    public class Persistence : IPersistence
    {
        private const String connectionString = "mongodb://localhost";
        private const String databaseName = "Persistence";
        private const String collectionName = "Messages";
        private MongoClient client;

        public bool SaveSession(String message)
        {
            client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<Message> collection = database.GetCollection<Message>(collectionName);

            long numDocuments = collection.Find(x => true).ToList().Count();

            collection.InsertOne(new Message { sessionID = (int) (numDocuments + 1), message = message });

            return true;
        }
        public List<String> RetrieveSession(int startSessionID, int endSessionID)
        {
            if (startSessionID > endSessionID)
            {
                Console.WriteLine("ERROR: startSessionID > endSessionID");  //change
                return null;
            }
            if(startSessionID<=0 || endSessionID <= 0)
            {
                Console.WriteLine("ERROR: Invalid value for sessionID");    //change
                return null;
            }
            
            client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<Message> collection = database.GetCollection<Message>(collectionName);
            List<String> returnMessages = new List<String>();

            long numDocuments = collection.Find(x => true).ToList().Count();

            for (int i=startSessionID; i <=endSessionID && i <= numDocuments; i++)
            {
                var result = collection.Find(x => x.sessionID == i).ToList()[0];
                returnMessages.Add(result.message);
            }

            return returnMessages;
            
        }
        public int DeleteSession(int startSessionID, int endSessionID)
        {
            if (startSessionID > endSessionID)
            {
                Console.WriteLine("ERROR: startSessionID > endSessionID");  //change
                return -1;
            }
            if (startSessionID <= 0 || endSessionID <= 0)
            {
                Console.WriteLine("ERROR: Invalid value for sessionID");    //change
                return -1;
            }

            client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<Message> collection = database.GetCollection<Message>(collectionName);

            int numDocumentsDeleted = 0;
            long numDocuments = collection.Find(x => true).ToList().Count();

            for (int i = startSessionID; i <= endSessionID && i <= numDocuments; i++)
            {
                DeleteResult result = collection.DeleteOne(x => x.sessionID == i);
                bool deleted = result.IsAcknowledged;
                
                if (deleted == true)
                {
                    numDocumentsDeleted += 1;
                }

            }

            if (endSessionID < numDocuments)
            {
                for(int i = endSessionID + 1; i <= numDocuments; i++)
                {
                    var filter = Builders<Message>.Filter.Eq("sessionID", i);
                    var update = Builders<Message>.Update.Set("sessionID", startSessionID-1+(i-endSessionID));
                    var result = collection.UpdateOne(filter, update);
                }
            }

            return numDocumentsDeleted;

        }
        public int GetCurrentSessionID()
        {
            client = new MongoClient(connectionString);
            IMongoDatabase database = client.GetDatabase(databaseName);
            IMongoCollection<Message> collection = database.GetCollection<Message>(collectionName);

            long numDocuments = collection.Find(x => true).ToList().Count();

            return ((int)numDocuments + 1);
        }
    }
    public class Message
    {
        public ObjectId _id { get; set; }
        public int sessionID { get; set; }
        public string message { get; set; }

    }
}
