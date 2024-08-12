using Amazon.DynamoDBv2.DataModel;
using System.ComponentModel.DataAnnotations;

namespace Lab3_NhatNguyen_WebApp.Models
{
    [DynamoDBTable("Movies")]
    public class Movie
    {
        [DynamoDBProperty("IMDBID")]
        public string IMDBID { get; set; }
        public string Title { get; set; }
        public List<string> Actors { get; set; }
        public string Description { get; set; }
        public List<string> Directors { get; set; }
        [DynamoDBProperty("Genre")]
        public List<string> Genre { get; set; }
        public string VideoKey { get;set; }
        public string ThumbnailKey { get; set; }
        public int Rating { get; set; }
        public int ReleasedTime { get; set; }
        public List<Review> Reviews { get; set; }
        public string UploadedBy { get; set; }
        public string Language { get; set; }
        public double IMDBRating { get;set; }
    }
}
