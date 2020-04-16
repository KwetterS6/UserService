using System;
using MongoDB.Bson.Serialization.Attributes;

namespace UserService.Models
{
    public class User
    {
        [BsonId]
        public Guid Id {get; set;}
        public string Name {get; set;}
        public string Email {get; set;}

        public byte[] Password { get; set; }

        public byte[] Salt { get; set; }

        public string Token { get; set; }
    }
}