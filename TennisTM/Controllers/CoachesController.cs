using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TennisTM.Data;
using TennisTM.Models;

namespace TennisTM.Controllers
{
    [Authorize]
    public class CoachesController : Controller
    {
        private readonly TennisTMDbContext dbContext;
        public CoachesController(TennisTMDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<IActionResult> Index(Guid? Id)
        {
            var coach = await dbContext.Coaches.Include(c => c.User).FirstOrDefaultAsync(x => x.Id == Id);
            if (coach != null) return View(coach);
            return NotFound();
        }
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || dbContext.Coaches == null)
            {
                return NotFound();
            }

            var coach = await dbContext.Coaches.FindAsync(id);
            if (coach == null)
            {
                return NotFound();
            }
            ViewData["UserId"] = new SelectList(dbContext.Users, "Id", "Id", coach.UserId);
            return View(coach);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Coach coachRes)
        {
            var coach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.Id == coachRes.Id);

            if(coach != null)
            {
                coach.Bio = coachRes.Bio;

                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index", new { coach.Id });
            }
            return View(coach);
        }
    }
}
