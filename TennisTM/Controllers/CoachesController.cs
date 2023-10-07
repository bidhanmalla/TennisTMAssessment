using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
            var coach = await dbContext.Coaches.Include(c => c.Schedules).ThenInclude(s => s.Users).Include(c => c.User).FirstOrDefaultAsync(x => x.Id == Id);
            if (coach != null) return View(coach);
            return NotFound();
        }
    }
}
