using Amazon.DynamoDBv2.DataModel;
using System.Runtime.Serialization;

namespace Lab3_NhatNguyen_WebApp.Models
{
    public class Review
    {
        public int ReviewId { get; set; }
        public string Email { get; set; }
        public string Comment { get; set; }
        public int UserRating { get;set; }
        public string Displayname { get; set; }
        public DateTime DateTime { get; set; }
    }
}
