using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorldOfTravels.Data;
using WorldOfTravels.Models;
using Microsoft.AspNetCore.Identity;

namespace WorldOfTravels.Controllers
{
    public class PostsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public PostsController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;
        }

        // GET: Posts
        public async Task<IActionResult> Index(string TitleSearchString, string DateSearch, int CountrySearch)
        {
            var posts = from d in _context.Post
                        select d;

            foreach (Post currPost in posts)
            {
                currPost.Country = _context.Country.First(c => c.ID == currPost.CountryID);
                currPost.Comments = _context.Comment.Where(c => c.PostID == currPost.ID).ToList();
            }

            if (!String.IsNullOrEmpty(TitleSearchString))
            {
                posts = posts.Where(p => p.Title.Contains(TitleSearchString));
            }

            if (!String.IsNullOrEmpty(DateSearch))
            {
                posts = posts.Where(p => p.PublishDate.Date == DateTime.Parse(DateSearch).Date);
            }

            if (CountrySearch != -1 && CountrySearch != 0)
            {
                posts = posts.Where(p => p.CountryID == CountrySearch);
            }

            PopulateCountriesSearchList();

            return View(await posts.OrderByDescending(p => p.PublishDate).ToListAsync());
        }

        // GET: Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }
            
            return View(post);
        }

        // GET: Posts/Create
        public IActionResult Create()
        {
            PopulateCountriesDropDownList();
            return View();
        }

        // POST: Posts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Content,Title,CountryID")] Post post)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                post.PublishDate = DateTime.Now;
                post.Country = (Country)(from d in _context.Country
                                     where d.ID == post.CountryID
                                     select d).First();

                post.UploaderUserName = HttpContext.User.Identity.Name;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public ActionResult PostComment(int postId, string content)
        {
            Comment comment = new Comment
            {
                Content = content,
                PostID = postId,
                CreationDate = DateTime.Now,
                UploaderUserName = HttpContext.User.Identity.Name
            };

            _context.Comment.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteComment(int id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            var comment = await _context.Comment.FindAsync(id);

            if (comment.UploaderUserName != loggedUser.UserName)
            {
                return RedirectToAction("Index");
            }

            _context.Comment.Remove(comment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }
            PopulateCountriesDropDownList(post.CountryID);
            return View(post);
        }

        // POST: Posts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Content,Title,PublishDate,CountryID")] Post post)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (id != post.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    post.PublishDate = DateTime.Now;
                    post.Country = (Country)(from d in _context.Country
                                             where d.ID == post.CountryID
                                             select d).First();
                    _context.Update(post);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateCountriesDropDownList(post.CountryID);
            return View(post);
        }

        // GET: Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Post
                .Include(p => p.Country)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (post == null)
            {
                return NotFound();
            }

            if (post.UploaderUserName != loggedUser.UserName)
            {
                return RedirectToAction("Index");
            }

            return View(post);
        }

        // POST: Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null)
            {
                return RedirectToAction("Index");
            }

            var post = await _context.Post.FindAsync(id);
            
            _context.Post.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Post.Any(e => e.ID == id);
        }

        private void PopulateCountriesDropDownList(object selectedCountry = null)
        {
            var countriesQuery = from d in _context.Country
                                   orderby d.Name
                                   select d;

            ViewBag.CountryID = new SelectList(countriesQuery, "ID", "Name", selectedCountry);
        }

        private void PopulateCountriesSearchList()
        {
            var countriesQuery = from d in _context.Country
                                 orderby d.Name
                                 select d;

            ViewBag.CountryID = new SelectList(countriesQuery, "ID", "Name", null);
        }
    }
}
