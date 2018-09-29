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
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;

        public UsersController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;

        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            var ApplicationUsers = await _manager.Users.ToListAsync<ApplicationUser>();
            List<User> UsersList = new List<User>();

            foreach (var user in ApplicationUsers)
            {
                User UserModel = new User { ID = user.Id, Username = user.UserName, IsAdmin = user.IsAdmin };
                UsersList.Add(UserModel);
            }

            return View(UsersList);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _manager.Users.FirstAsync<ApplicationUser>(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new User { ID = user.Id, Username = user.UserName, IsAdmin = user.IsAdmin});
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ID,Username,IsAdmin")] User user)
        {
            if (id != user.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser oldUser = await _manager.Users.FirstAsync<ApplicationUser>(u => u.Id == id);
                    oldUser.UserName = user.Username;
                    oldUser.IsAdmin = user.IsAdmin;
                    oldUser.Email = user.Username;
                    IdentityResult result = await _manager.UpdateAsync(oldUser);

                    if (!result.Succeeded)
                    {
                        
                        throw new DbUpdateConcurrencyException("Save failed", null);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.ID))
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
            return View(user);
        }

        // GET: Users/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (String.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            if (id == _manager.GetUserId(User))
            {
                return RedirectToAction(nameof(Index));
            }

            var user = await _manager.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(new User { ID = id, Username = user.UserName, IsAdmin = user.IsAdmin});
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _manager.Users
               .FirstOrDefaultAsync(m => m.Id == id);
            await _manager.DeleteAsync(user);
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(string id)
        {
            return _manager.Users.Any<ApplicationUser>(e => e.Id == id);
        }
    }
}
