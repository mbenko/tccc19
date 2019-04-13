using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using myFeedback.Models;

namespace myFeedback.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly myDataContext _context;
        private readonly CosmosContainer _cosmos;

        public FeedbackController(myDataContext context, CosmosContainer cosmos)
        {
            _cosmos = cosmos;
            _context = context;
        }

        // GET: Feedback
        public async Task<IActionResult> Index()
        {
            return View(await GetCosmosItems());
            return View(await _context.Feedback.ToListAsync());
        }

        // Return List of Feedback from Cosmos DB
        private async Task<List<Feedback>> GetCosmosItems()
        {
            // Read a single query page from Azure cosmos DB as stream
            var myQueryDef = new CosmosSqlQueryDefinition($"Select * from f where f.Complete != true");

            var myQuery = _cosmos.Items.CreateItemQuery<Feedback>(myQueryDef, maxConcurrency: 4);

            List<Feedback> myData = new List<Feedback>();
            while (myQuery.HasMoreResults)
            {
                var set = await myQuery.FetchNextSetAsync();
                myData.AddRange(set);
            }
            return myData;
        }


        // GET: Feedback/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Feedback/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Issue,Session,Complete,CreatedAt")] Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                // Add to SQL
                _context.Add(feedback);
                await _context.SaveChangesAsync();

                // Add to Cosmos
                await _cosmos.Items.CreateItemAsync<Feedback>(feedback.Session, feedback);

                return RedirectToAction(nameof(Index));
            }
            return View(feedback);
        }

        // GET: Feedback/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback.FindAsync(id);
            if (feedback == null)
            {
                return NotFound();
            }
            return View(feedback);
        }

        // POST: Feedback/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Issue,Session,Complete,CreatedAt")] Feedback feedback)
        {
            if (id != feedback.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(feedback);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FeedbackExists(feedback.Id))
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
            return View(feedback);
        }

        // GET: Feedback/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // GET: Feedback/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var feedback = await _context.Feedback
                .FirstOrDefaultAsync(m => m.Id == id);
            if (feedback == null)
            {
                return NotFound();
            }

            return View(feedback);
        }

        // POST: Feedback/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var feedback = await _context.Feedback.FindAsync(id);
            _context.Feedback.Remove(feedback);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FeedbackExists(string id)
        {
            return _context.Feedback.Any(e => e.Id == id);
        }
    }
}
