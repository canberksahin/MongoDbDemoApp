using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace MongoDbDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            MongoCRUD db = new MongoCRUD("AddressBook");

            //PersonModel person = new PersonModel
            //{
            //    FirstName = "Aysenur",
            //    LastName = "Şahin",
            //    PrimaryAddress = new AddressModel
            //    {
            //        StreetAdress = "Nazilli",
            //        State = "Türkiye",
            //        City = "Ankara",
            //        Zipcode = "06540"
            //    }
            //};
            //PersonModel person2 = new PersonModel
            //{
            //    FirstName = "Yeni",
            //    LastName = "Şahin"
            //};

            //db.InsertRecord("Users", person);
            //db.InsertRecord("Users", person2);

            var recs = db.LoadRecords<NameModel>("Users");
            foreach (var rec in recs)
            {
                Console.WriteLine($" {rec.FirstName} {rec.LastName}");

                Console.WriteLine();
            }



            //var oneRec = db.LoadRecordById<PersonModel>("Users",new Guid("5f373d47-5cfd-41df-a1b7-f80cd340180c"));
            //Console.WriteLine(oneRec.FirstName);

            ////UPDATE
            //oneRec.DateOfBirth = DateTime.Now;
            //db.UpsertRecord("Users", oneRec.Id, oneRec);


            ////DELETE
            //db.DeleteRecord<PersonModel>("Users", oneRec.Id);
            Console.ReadLine();
        }
    }

    public class NameModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

        public class PersonModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public AddressModel PrimaryAddress { get; set; }

        [BsonElement("dob")]
        public DateTime DateOfBirth { get; set; }
    }

    public class AddressModel
    {
        public string  StreetAdress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zipcode { get; set; }
    }

    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string database)
        {
            var client = new MongoClient();
            db = client.GetDatabase(database);
        }

        public void InsertRecord<T>(string table, T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecords<T>(string table)
        {
            var collection = db.GetCollection<T>(table);
            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id",id);

            return collection.Find(filter).First();
        }

        public void UpsertRecord<T>(string table, Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var result = collection.ReplaceOne(
                new BsonDocument("_id", id),
                record,
                new UpdateOptions { IsUpsert = true });
        }

        public void DeleteRecord<T> (string table, Guid id)
        {
            var collection = db.GetCollection<T>(table);
            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }
    }
}
