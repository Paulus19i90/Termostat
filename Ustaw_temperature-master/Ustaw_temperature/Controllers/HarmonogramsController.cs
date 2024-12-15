using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Ustaw_temperature.Data;
using Ustaw_temperature.Models;

namespace Ustaw_temperature.Controllers
{
    public class HarmonogramsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HarmonogramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Harmonograms
        public async Task<IActionResult> Index()
        {
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uzytkownikAktualny == null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            return View(await _context.Harmonogram.ToListAsync());
        }

        // GET: Harmonograms/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uzytkownikAktualny == null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }

            var harmonogram = await _context.Harmonogram
                .FirstOrDefaultAsync(m => m.Id == id);
            if (harmonogram == null)
            {
                return NotFound();
            }

            return View(harmonogram);
        }

        // GET: Harmonograms/Create
        public IActionResult Create()

        {
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uzytkownikAktualny == null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            return View();
        }

        // POST: Harmonograms/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nazwa,Start,End,DocelowaTemperatura")] Harmonogram harmonogram)
        {
            if (ModelState.IsValid)
            {
                // Pobierz identyfikator zalogowanego użytkownika
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return Unauthorized(); // Jeśli użytkownik nie jest zalogowany
                }

                // Znajdź mieszkanie związane z użytkownikiem
                var mieszkanie = await _context.Mieszkanie
                    .FirstOrDefaultAsync(m => m.Uzytkownik == userId || m.Uzytkownik2 == userId);

                if (mieszkanie == null)
                {
                    return NotFound("Nie znaleziono mieszkania przypisanego do użytkownika.");
                }

                // Ustaw klucz obcy
                harmonogram.MieszkanieId = mieszkanie.Id;

                // Zapisz harmonogram
                _context.Add(harmonogram);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(harmonogram);
        }

        // GET: Harmonograms/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var harmonogram = await _context.Harmonogram
                                             .Include(h => h.Mieszkanie) // Zakładając, że chcesz również załadować powiązane mieszkanie
                                             .FirstOrDefaultAsync(h => h.Id == id);

            if (harmonogram == null)
            {
                return NotFound();
            }

            // Przekazujemy dane mieszkania, ale nie edytujemy ich w formularzu
            ViewBag.Mieszkania = _context.Mieszkanie.ToList();

            return View(harmonogram);
        }

        // POST: Harmonograms/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nazwa,Start,End,DocelowaTemperatura")] Harmonogram harmonogram)
        {
            if (id != harmonogram.Id)
            {
                return NotFound();
            }

            var existingHarmonogram = await _context.Harmonogram.AsNoTracking().FirstOrDefaultAsync(h => h.Id == id);
            if (existingHarmonogram == null)
            {
                return NotFound();
            }

            // Odłączamy poprzednią instancję, jeśli jest już śledzona
            var entry = _context.Entry(existingHarmonogram);
            entry.State = EntityState.Detached;

            harmonogram.MieszkanieId = existingHarmonogram.MieszkanieId; // MieszkanieId pozostaje niezmienne
            _context.Harmonogram.Update(harmonogram);

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania zmian.");
                    return View(harmonogram);
                }
            }
            return View(harmonogram);
        }

        // GET: Harmonograms/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uzytkownikAktualny == null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }

            var harmonogram = await _context.Harmonogram
                .FirstOrDefaultAsync(m => m.Id == id);
            if (harmonogram == null)
            {
                return NotFound();
            }

            return View(harmonogram);
        }

        // POST: Harmonograms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var harmonogram = await _context.Harmonogram.FindAsync(id);
            if (harmonogram != null)
            {
                _context.Harmonogram.Remove(harmonogram);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HarmonogramExists(int id)
        {
            return _context.Harmonogram.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Licznik(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var uzytkownikAktualny = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (uzytkownikAktualny == null)
            {
                return Forbid(); // Zwraca 403 Forbidden
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            // Pobierz dane mieszkania
            var mieszkanie = await _context.Mieszkanie
                    .FirstOrDefaultAsync(m => m.Uzytkownik == userId || m.Uzytkownik2 == userId);
            if (mieszkanie == null)
            {
                return NotFound("Mieszkanie nie zostało znalezione.");
            }

            // Pobierz dane harmonogramu
            var harmonogram = await _context.Harmonogram.FindAsync(id);
            if (harmonogram == null)
            {
                return NotFound("Harmonogram nie został znaleziony.");
            }

            // Oblicz cenę dzienną
            decimal kurs = 0.1m;
            decimal liczbaOkien = (decimal)mieszkanie.LiczbaOkien;
            decimal liczbaPokoi = (decimal)mieszkanie.LiczbaPokoi;
            decimal bazowaTemperatura = (decimal)mieszkanie.BazowaTemperatura;
            decimal docelowaTemperatura = (decimal)harmonogram.DocelowaTemperatura;
            decimal czasTrwania = (decimal)harmonogram.End - (decimal)harmonogram.Start;

            decimal cenaDzienna = Math.Round(
                (liczbaOkien * 1.2m)
                * (liczbaPokoi * 1.5m)
                * czasTrwania
                * ((docelowaTemperatura - bazowaTemperatura))
                * kurs,
                2, 
                MidpointRounding.AwayFromZero 
            );

            decimal cenatygodniowa = Math.Round(cenaDzienna * 7, 2, MidpointRounding.AwayFromZero);
            decimal cenamiesieczna = Math.Round(cenaDzienna * 30, 2, MidpointRounding.AwayFromZero);
            ViewBag.CenaDzienna = cenaDzienna;
            ViewBag.cenatygodniowa = cenatygodniowa;
            ViewBag.cenamiesieczna = cenamiesieczna;
            return View();
        }
    }
    
}
