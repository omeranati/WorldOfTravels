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
using DataAccess;
using System.Configuration;
using libsvm;

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

                post.UploaderUsername = HttpContext.User.Identity.Name;

                _context.Add(post);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(post);
        }

        public ActionResult PostComment(int postId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                Comment comment = new Comment
                {
                    Content = content,
                    PostID = postId,
                    CreationDate = DateTime.Now,
                    UploaderUsername = HttpContext.User.Identity.Name
                };

                _context.Comment.Add(comment);
                _context.SaveChanges();
            }

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

            if (comment.UploaderUsername != loggedUser.UserName)
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
                    post.UploaderUsername = HttpContext.User.Identity.Name;
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

            if (post.UploaderUsername != loggedUser.UserName)
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

        // GET: Posts/GroupByCountry
        public async Task<ActionResult> Graphs()
        {
            var query = from post in _context.Post
                        group post by post.CountryID into p
                        join country in _context.Country on p.Key equals country.ID
                        select new GroupByCountry() { CountryName = country.Name, TotalPosts = p.Sum(s => 1) };

            return View(await query.OrderByDescending(p => p.TotalPosts).ToListAsync());
        }

        [HttpGet]
        public ActionResult GetGroupByCountry()
        {
            var query = from post in _context.Post
                        group post by post.Country.Name into g
                        select new GroupByCountry() { CountryName = g.Key, TotalPosts = g.Sum(p => 1) };

            return Json(query);
        }

        [HttpGet]
        public ActionResult GetCommentsGroupByPost()
        {
            var query = from comment in _context.Comment
                        group comment by comment.PostID into c
                        join post in _context.Post on c.Key equals post.ID
                        select new { Name = post.Title, Count = c.Sum(p => 1) };

            return Json(query.OrderByDescending(p => p.Count));
        }

        public IActionResult RecommendedDestinations()
        {
            // Load the predifined data for smv algorithm
            string dataFilePath = "./wwwroot/svm/postsData.csv";
            var dataTable = DataTable.New.ReadCsv(dataFilePath);
            List<string> data = dataTable.Rows.Select(row => row["Text"]).ToList();

            // Load classes (-1 or +1)
            double[] classes = dataTable.Rows.Select(row => double.Parse(row["IsRecommended"]))
                                       .ToArray();

            // Get words
            var vocabulary = data.SelectMany(GetWords).Distinct().OrderBy(word => word).ToList();

            // Generate a svm problem
            var problem = CreateProblem(data, classes, vocabulary.ToList());

            // Create and train a smv model
            const int C = 1;
            var model = new libsvm.C_SVC(problem, KernelHelper.LinearKernel(), C);

            Dictionary<int, string> _predictionDictionary = new Dictionary<int, string> { { -1, "NotRecommended" }, { 1, "Recommended" } };

            // Get all posts
            var posts = _context.Post.ToList();

            // Get recommended posts
            foreach (Post currPost in posts)
            {
                if (currPost.Content != null)
                {
                    var node = CreateNode(currPost.Content, vocabulary);
                    var prediction = model.Predict(node);

                    currPost.IsRecommended = _predictionDictionary[(int)prediction] == "Recommended";
                }
                else
                {
                    currPost.IsRecommended = false;
                }
            }

            var recommendedPosts = posts.Where(p => p.IsRecommended == true);

            foreach (Post currPost in recommendedPosts)
            {
                currPost.Country = _context.Country.First(c => c.ID == currPost.CountryID);
                currPost.Comments = _context.Comment.Where(c => c.PostID == currPost.ID).ToList();
            }

            return View(recommendedPosts.OrderByDescending(p => p.PublishDate));
        }

        private static IEnumerable<string> GetWords(string x)
        {
            return x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        }

        private svm_problem CreateProblem(IEnumerable<string> x, double[] y, IReadOnlyList<string> vocabulary)
        {
            return new svm_problem
            {
                y = y,
                x = x.Select(xVector => CreateNode(xVector, vocabulary)).ToArray(),
                l = y.Length
            };
        }

        private static svm_node[] CreateNode(string x, IReadOnlyList<string> vocabulary)
        {
            var node = new List<svm_node>(vocabulary.Count);

            string[] words = x.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < vocabulary.Count; i++)
            {
                int occurenceCount = words.Count(s => String.Equals(s, vocabulary[i], StringComparison.OrdinalIgnoreCase));
                if (occurenceCount == 0)
                    continue;

                node.Add(new svm_node
                {
                    index = i + 1,
                    value = occurenceCount
                });
            }

            return node.ToArray();
        }
    }
}