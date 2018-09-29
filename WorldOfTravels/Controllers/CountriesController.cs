﻿using System;
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
    public class CountriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _manager;
        public CountriesController(ApplicationDbContext context, UserManager<ApplicationUser> manager)
        {
            _context = context;
            _manager = manager;

        }

        // GET: Countries
        public async Task<IActionResult> Index(string NameSearch, string ContinentSearch, string TropicalSearch)
        {
            var countries = from d in _context.Country
                            select d;

            if (!String.IsNullOrEmpty(NameSearch))
            {
                countries = countries.Where(p => p.Name.Contains(NameSearch));
            }

            if (!String.IsNullOrEmpty(ContinentSearch) && !ContinentSearch.Equals("All"))
            {
                var continentName = (Continent)Convert.ToInt32(ContinentSearch);
                countries = countries.Where(p => p.Continent.Equals(continentName));
            }

            if (!String.IsNullOrEmpty(TropicalSearch) && !TropicalSearch.Equals("All"))
            {
                var isTropical = TropicalSearch.Equals("Yes");
                countries = countries.Where(p => p.IsTropical == isTropical);
            }
            
            return View(await countries.ToListAsync());
        }

        // GET: Countries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.ID == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // GET: Countries/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Countries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Continent,IsTropical")] Country country)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(country);
        }

        // GET: Countries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.FindAsync(id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        // POST: Countries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Continent,IsTropical")] Country country)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            if (id != country.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction("Index");
            }
            return View(country);
        }

        // GET: Countries/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country
                .FirstOrDefaultAsync(m => m.ID == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        // POST: Countries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loggedUser = await _manager.GetUserAsync(User);

            if (loggedUser == null || !loggedUser.IsAdmin)
            {
                return RedirectToAction("Index");
            }

            var country = await _context.Country.FindAsync(id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CountryExists(int id)
        {
            return _context.Country.Any(e => e.ID == id);
        }
    }
}
