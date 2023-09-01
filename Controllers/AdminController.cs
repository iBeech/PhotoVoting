using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhotovotingApp.Models;
using System.Linq;

namespace PhotovotingApp.Controllers
{
    public class AdminController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly AppDbContext _dbContext;

        public AdminController(IWebHostEnvironment webHostEnvironment,AppDbContext dbContext)
        {
            _webHostEnvironment = webHostEnvironment;
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            var photos = _dbContext.Photos.OrderByDescending(p => p.Votes).ToList();
            return View(photos);
        }

        public IActionResult DeleteAllPhotos()
        {
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            var uploadsDirectory = new DirectoryInfo(uploadsFolder);
            foreach (var file in uploadsDirectory.EnumerateFiles("*.*", SearchOption.AllDirectories))
            {
                try
                {
                    Console.WriteLine($"Deleting file {file.FullName}");
                    file.Delete();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }

            _dbContext.Photos.RemoveRange(_dbContext.Photos);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {            
            var photo = _dbContext.Photos.Find(id);
            if (photo == null) return RedirectToAction("Index");

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
            string pathToFile = Path.Combine(uploadsFolder, photo.ImageFileName);
            new FileInfo(pathToFile).Delete();
            _dbContext.Photos.Remove(photo);

            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int id, string title, string author)
        {
            // Retrieve the photo by its ID from the database
            var photo = _dbContext.Photos.FirstOrDefault(p => p.Id == id);

            if (photo == null)
            {
                // Handle the case where the photo doesn't exist
                return NotFound();
            }

            // Update the title and author properties of the photo
            if(!string.IsNullOrEmpty(title)) photo.Title = title;
            if(!string.IsNullOrEmpty(author)) photo.Author = author;

            // Save the changes to the database
            _dbContext.SaveChanges();

            // Redirect back to the Admin page (or any other appropriate action)
            return RedirectToAction("Index"); // Replace with your admin page action
        }



        public IActionResult ResetPhotoVotes(int id)
        {

            var photo = _dbContext.Photos.Find(id); 
            if (photo == null) return NotFound();

            photo.Votes = 0;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public IActionResult ResetVotes()
        {

            _dbContext.Photos.ForEachAsync(p =>
            {
                p.Votes = 0;
            });
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}