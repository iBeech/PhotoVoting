using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotovotingApp.Models;
using PhotoVotingApp.Models;

namespace PhotoVotingApp.Controllers
{
    public class PhotoController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _dbContext;

        public PhotoController(IWebHostEnvironment webHostEnvironment, AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var photos = _dbContext.Photos.ToList();

            if (photos.Count == 0) return RedirectToAction("Upload");

            // Create a random seed based on the current time
            var randomSeed = (int)DateTime.Now.Ticks;

            // Randomly shuffle the photos
            var random = new Random(randomSeed);
            var shuffledPhotos = photos.OrderBy(_ => random.Next()).ToList();

            return View(shuffledPhotos);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Upload(Photo photo, IFormFile imageFile)
        {
            if (ModelState.IsValid && imageFile != null && imageFile.Length > 0)
            {
                // Process and save the image
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads"); // Assuming "uploads" is a folder under wwwroot

                // Extension of the file on disk                
                var sourceFile = new FileInfo(imageFile.FileName);
                var uniqueFileName = $"{Guid.NewGuid()}{sourceFile.Extension}";
                string destinationFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(destinationFilePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // Update the Photo entity with the image file name
                photo.ImageFileName = uniqueFileName;

                // Save the photo entity to the database
                _dbContext.Photos.Add(photo);
                _dbContext.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(photo);
        }

        [HttpPost]
        public IActionResult Vote(int id)
        {
            var photo = _dbContext.Photos.Find(id);

            if (photo != null)
            {
                // Check if the user has already voted by checking the presence of a cookie
                if (!HasUserVotedCookie())
                {
                    // Increment the vote count
                    photo.Votes++;
                    _dbContext.SaveChanges();

                    // Set a cookie to mark that the user has voted
                    SetUserVotedCookie(id);

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        private bool HasUserVotedCookie()
        {
            return Request.Cookies.ContainsKey("HasVoted");
        }

        private int GetUserVotedPhotoId()
        {
            try
            {
                return int.Parse(s: Request.Cookies["HasVoted"]);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        private void SetUserVotedCookie(int photoId)
        {
            // Set a cookie to mark that the user has voted
            Response.Cookies.Append("HasVoted", photoId.ToString());
        }

        [HttpPost]
        public IActionResult ResetVote()
        {
            // Clear the voted cookie
            DeleteUserVotedCookie();

            
            var photoId = GetUserVotedPhotoId();

            // Incase we have invalid data in our cookie
            if(photoId == -1) { return View("Index"); }

            var photo = _dbContext.Photos.Find(photoId);

            if (photo != null)
            {
                if (photo.Votes > 0)
                {
                    photo.Votes--;
                    _dbContext.SaveChanges();
                }

                DeleteUserVotedCookie();
            }

            return RedirectToAction("Index");
        }

        private void DeleteUserVotedCookie()
        {
            Response.Cookies.Delete("HasVoted");
        }
    }
}
