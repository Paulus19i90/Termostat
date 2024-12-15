using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ustaw_temperature.Data;
using Ustaw_temperature.Models;

namespace Ustaw_temperature.Controllers
{
    public class MieszkaniesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MieszkaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mieszkanies
        public async Task<IActionResult> Index()
        {
            
            string urzytkonikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var czy_jest = await _context.Mieszkanie
                .Where(s => s.Uzytkownik == urzytkonikId || s.Uzytkownik2 == urzytkonikId).FirstOrDefaultAsync();
            ViewData["test"] = czy_jest;
            
            return View(await _context.Mieszkanie
                .Include(s => s.User)
                .Where(s => s.Uzytkownik == urzytkonikId || s.Uzytkownik2 == urzytkonikId)
                .ToListAsync());
        }

        // GET: Mieszkanies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var mieszkanie = await _context.Mieszkanie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mieszkanie == null)
            {
                return NotFound();
            }

            return View(mieszkanie);
        }

        // GET: Mieszkanies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Mieszkanies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LiczbaOkien,LiczbaPokoi,BazowaTemperatura")] Mieszkanie mieszkanie)
        {
            var urzytkonikId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var istniejąceMieszkanie = await _context.Mieszkanie
        .Where(m => (m.Uzytkownik == urzytkonikId || m.Uzytkownik2 == urzytkonikId) && m.Id != mieszkanie.Id)
        .FirstOrDefaultAsync();

            if (istniejąceMieszkanie != null)
            {
                // Dodaj komunikat błędu, jeśli użytkownik już jest przypisany do innego mieszkania
                ModelState.AddModelError("", "Ale ty już masz mieszkanie, po co drugie?");
                return View(mieszkanie);
            }
            mieszkanie.Uzytkownik = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid)
            {
                _context.Add(mieszkanie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mieszkanie);
        }

        // POST: Mieszkanies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mieszkanie = await _context.Mieszkanie.FindAsync(id);
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (mieszkanie.Uzytkownik2 == uzytkownikAktualny)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            if (mieszkanie == null)
            {
                return NotFound();
            }
            return View(mieszkanie);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LiczbaOkien,LiczbaPokoi,BazowaTemperatura")] Mieszkanie mieszkanie)
        {
            if (id != mieszkanie.Id)
            {
                return NotFound();
            }

            // Odłączanie obiektu od śledzenia, aby uniknąć konfliktu
            var mieszkanie1 = await _context.Mieszkanie.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (mieszkanie1 == null)
            {
                return NotFound();
            }

            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (mieszkanie1.Uzytkownik2 == uzytkownikAktualny)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }

            // Zachowanie danych użytkownika
            mieszkanie.Uzytkownik = mieszkanie1.Uzytkownik;
            mieszkanie.Uzytkownik2 = mieszkanie1.Uzytkownik2;

            if (ModelState.IsValid)
            {
                try
                {
                    // Oznaczanie obiektu jako zmodyfikowanego
                    _context.Entry(mieszkanie).State = EntityState.Modified;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MieszkanieExists(mieszkanie.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            return View(mieszkanie);
        }

        // GET: Mieszkanies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var mieszkanie = await _context.Mieszkanie
                .FirstOrDefaultAsync(m => m.Id == id);
            if (id == null)
            {
                return NotFound();
            }
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (mieszkanie.Uzytkownik2 == uzytkownikAktualny)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }

            
            if (mieszkanie == null)
            {
                return NotFound();
            }

            return View(mieszkanie);
        }

        // POST: Mieszkanies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mieszkanie = await _context.Mieszkanie.FindAsync(id);
            if (mieszkanie != null)
            {
                _context.Mieszkanie.Remove(mieszkanie);
            }
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (mieszkanie.Uzytkownik2 == uzytkownikAktualny)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MieszkanieExists(int id)
        {
            return _context.Mieszkanie.Any(e => e.Id == id);
        }
        public async Task<IActionResult> Dodaj(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mieszkanie = await _context.Mieszkanie.FindAsync(id);
            if (mieszkanie == null)
            {
                return NotFound();
            }
            
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (mieszkanie.Uzytkownik2 == uzytkownikAktualny)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            if (uzytkownikAktualny==null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            var przypisaniUzytkownicy = _context.Mieszkanie
            .Select(d => d.Uzytkownik)
            .ToList();
            var filteredUsers = await _context.Users
            .Where(user => user.Id != uzytkownikAktualny && !przypisaniUzytkownicy.Contains(user.Id))
            .ToListAsync();

            ViewData["Uzytkownik2"] = new SelectList(filteredUsers, "Id", "UserName");

            return View(mieszkanie);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(int id, string Uzytkownik2 )
        {
            var mieszkanie = await _context.Mieszkanie.FindAsync(id);
            if (mieszkanie == null)
            {
                return NotFound();
            }

            // Zaktualizuj tylko właściwość Uzytkownik2
            mieszkanie.Uzytkownik2 = Uzytkownik2;

            // Zapisz zmiany
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MieszkanieExists(id))
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
            
        
    }
}
