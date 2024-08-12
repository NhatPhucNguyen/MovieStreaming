using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Azure.Core;
using Lab3_NhatNguyen_WebApp.Models;
using Lab3_NhatNguyen_WebApp.Services;
using Lab3_NhatNguyen_WebApp.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;

namespace Lab3_NhatNguyen_WebApp.Controllers
{
    public class MoviesController : Controller
    {
        private DynamoDBContext dbContext;
        private readonly AmazonCredential _credential;
        public MoviesController(AmazonCredential credential)
        {
            _credential = credential;
            AmazonDynamoDBManagement.InitializeClient(_credential.AccessKey,_credential.SecretKey);
            AmazonS3Management.InitializeClient(_credential.AccessKey, _credential.SecretKey);
            dbContext = new DynamoDBContext(AmazonDynamoDBManagement.client);
        }
        public async Task<IActionResult> Index(string genre,int rate)
        {
            
            var claimUser = HttpContext.User;
            if (!claimUser.Identity.IsAuthenticated)
            {
                return Redirect("/");
            }

            try
            {
                var movies = new List<Movie>();
                if(genre != null && rate > 0)
                {
                    movies = await dbContext.ScanAsync<Movie>(new List<ScanCondition>()
                    {
                        new ScanCondition("Genre",ScanOperator.Contains,genre),
                        new ScanCondition("Rating",ScanOperator.GreaterThan,rate)
                    }).GetRemainingAsync();
                }
                else if(genre != null)
                {
                    movies = await dbContext.ScanAsync<Movie>(new List<ScanCondition>()
                    {
                        new ScanCondition("Genre",ScanOperator.Contains,genre)
                    }).GetRemainingAsync();
                }
                else if(rate > 0)
                {
                    var scanConfig = new DynamoDBOperationConfig()
                    {
                        IndexName = "Rating-Index",

                    };
                    var scanCondition = new List<ScanCondition>()
                    {
                        new ScanCondition("Rating",ScanOperator.GreaterThan,rate)
                    };
                    movies = await dbContext.ScanAsync<Movie>(scanCondition, scanConfig).GetRemainingAsync();
                }
                else
                {
                    movies = await dbContext.ScanAsync<Movie>(new List<ScanCondition>()).GetRemainingAsync();
                }
                ViewBag.Movies = movies;
                ViewBag.Genre = genre;
                return View();
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/movies/{id}")]
        public async Task<IActionResult> Detail(string id)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search.Count > 0)
                {
                    var movie = search.First();
                    movie.Reviews.Sort((x, y) => y.DateTime.CompareTo(x.DateTime));
                    ViewBag.Movie = movie;
                    return View("Detail");
                }
                return Redirect("/");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("/movies/{id}")]
        public async Task<IActionResult> SubmitReview(Review review,string id)
        {

            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search.Count > 0)
                {
                    var movie = search.First();
                    var userEmail = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    var displayName = HttpContext.User.Identity.Name;
                    review.Email = userEmail;
                    review.Displayname = displayName;
                    review.DateTime = DateTime.Now;
                    review.ReviewId = movie.Reviews.Count;
                    movie.Reviews.Add(review);
                    if (movie.Reviews.Count > 0)
                    {
                        var averageRating = movie.Reviews.Sum(x => x.UserRating) / movie.Reviews.Count;
                        movie.Rating = averageRating;
                    }
                    await dbContext.SaveAsync<Movie>(movie);
                    ViewBag.Movie = movie;
                    return Redirect($"/movies/{movie.IMDBID}");
                }
                return Redirect("/");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/movies/watch/{id}")]
        public async Task<IActionResult> WatchMovie(string id)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search.Count > 0)
                {
                    var movie = search.First();
                    ViewBag.Src = $"https://moviesdatabucket.s3.ca-central-1.amazonaws.com/{movie.VideoKey}";
                    ViewBag.Poster = $"https://moviesdatabucket.s3.ca-central-1.amazonaws.com/{movie.ThumbnailKey}";
                    ViewBag.Movie = movie;
                    return View("Watch");
                }
                return Redirect("/");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("/movies/{id}/review/{reviewId}")]
        public async Task<IActionResult> EditReview(Review review, string id, string reviewId)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search.Count > 0)
                {
                    var movie = search.First();
                    var foundReview = movie.Reviews.Find(x => x.ReviewId.ToString() == reviewId);
                    if(foundReview != null)
                    {
                        foundReview.UserRating = review.UserRating;
                        foundReview.Comment = review.Comment;
                        foundReview.DateTime = DateTime.Now;
                    }
                    var averageRating = movie.Reviews.Sum(x => x.UserRating) / movie.Reviews.Count;
                    movie.Rating = averageRating;
                    await dbContext.SaveAsync<Movie>(movie);
                    ViewBag.Movie = movie;
                    return Redirect($"/movies/{id}");
                }
                return Redirect("/");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/movies/{id}/delete/{reviewId}")]
        public async Task<IActionResult> DeleteReview(Review review, string id, string reviewId)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search.Count > 0)
                {
                    var movie = search.First();
                    var foundReview = movie.Reviews.Find(x => x.ReviewId.ToString() == reviewId);
                    if (foundReview != null)
                    {
                        movie.Reviews.Remove(foundReview);
                    }
                    await dbContext.SaveAsync<Movie>(movie);
                    ViewBag.Movie = movie;
                    return Redirect($"/movies/{id}");
                }
                return Redirect("/");
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Access movies uploaded by user
        [Authorize]
        [Route("/admin/movies")]
        public async Task<IActionResult> YourMovies()
        {
            try
            {
                var currentUser = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var movies = await dbContext.ScanAsync<Movie>(new List<ScanCondition> { new ScanCondition("UploadedBy", ScanOperator.Contains, currentUser) }).GetRemainingAsync();
                ViewBag.Movies = movies;
                ViewBag.User = currentUser;
                return View("Index");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/admin/movies/upload")]

        public async Task<IActionResult> Upload()
        {
            try
            {                
                return View("Upload");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("/admin/movies/upload")]
        public async Task<IActionResult> UploadSubmit(MovieFormModel movieForm,IFormFile videoPath, IFormFile thumbnailPath)
        {
            try
            {                
                if(videoPath != null)
                {
                    //file uploading
                    bool isUploaded = await AmazonS3Management.UploadFile(videoPath, movieForm.IMDBID);
                    if (isUploaded)
                    {                        
                        if(thumbnailPath != null)
                        {
                            await AmazonS3Management.UploadFile(thumbnailPath, movieForm.IMDBID);
                        }
                    }
                    //save movie
                    Movie newMovie = movieForm;
                    newMovie.UploadedBy = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
                    newMovie.VideoKey = newMovie.IMDBID + Path.GetExtension(videoPath.FileName);
                    if (thumbnailPath != null)
                    {
                        newMovie.ThumbnailKey = newMovie.IMDBID + Path.GetExtension(thumbnailPath.FileName);
                    };
                    newMovie.Actors = HandleListInput(movieForm.Actor);
                    newMovie.Directors = HandleListInput(movieForm.Director);
                    newMovie.Genre = new List<string>() { movieForm.MainGenre, movieForm.SecondGenre };
                    newMovie.Reviews = new List<Review> { };
                    await dbContext.SaveAsync(newMovie);
                    ViewBag.Success = "Your movie uploaded successfully";
                    return View("Upload");
                }
                ViewBag.Error = "Please select the video file";
                return View("Upload");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/admin/movies/upload/{id}")]
        public async Task<IActionResult> MovieEdit(string id)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                if (search != null && search.Count > 0)
                {
                    var movie = search.First();
                    ViewBag.Movie = movie;
                }
                return View("Upload");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [HttpPost]
        [Route("/admin/movies/upload/{id}")]
        public async Task<IActionResult> MovieEditSubmit(MovieFormModel movieForm, IFormFile videoPath, IFormFile thumbnailPath,string id)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                var movieToUpdate = search.First();
                if(thumbnailPath != null)
                {
                    await AmazonS3Management.UploadFile(thumbnailPath, movieToUpdate.IMDBID);
                    movieToUpdate.ThumbnailKey = movieToUpdate.IMDBID + Path.GetExtension(thumbnailPath.FileName);
                }
                if (videoPath != null)
                {
                    await AmazonS3Management.UploadFile(videoPath, movieToUpdate.IMDBID);
                    movieToUpdate.VideoKey = movieToUpdate.IMDBID + Path.GetExtension(videoPath.FileName);
                }
                movieToUpdate.Actors = HandleListInput(movieForm.Actor);
                movieToUpdate.Directors = HandleListInput(movieForm.Director);
                movieToUpdate.Genre = new List<string>() { movieForm.MainGenre,movieForm.SecondGenre };
                movieToUpdate.IMDBRating = movieForm.IMDBRating;
                movieToUpdate.Description = movieForm.Description;
                movieToUpdate.Title = movieForm.Title;
                await dbContext.SaveAsync(movieToUpdate);                
                return Redirect("/admin/movies");
            }
            catch (Exception)
            {
                throw;
            }
        }
        [Authorize]
        [Route("/admin/movies/delete/{id}")]
        public async Task<IActionResult> MovieDelete(string id)
        {
            try
            {
                var search = await dbContext.QueryAsync<Movie>(id).GetRemainingAsync();
                var movieToDelete = search.First();
                await AmazonS3Management.DeleteObject(movieToDelete.VideoKey);
                await AmazonS3Management.DeleteObject(movieToDelete.ThumbnailKey);
                await dbContext.DeleteAsync(movieToDelete);
                return Redirect("/admin/movies");
            }
            catch (Exception)
            {
                throw;
            }
        }
        private List<string> HandleListInput(string input)
        {
            var list = new List<string>();
            foreach ( var item in input.Split(",") )
            {
                list.Add(item.Trim());
            }
            return list;
        }
    }
}
