using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Metrics;
using TennisTM.Data;
using TennisTM.Models;

namespace TennisTM.Controllers
{
    public class MembersController : Controller
    {
        private readonly TennisTMDbContext dbContext;
        private readonly UserManager<User> userManager;
        //private readonly SignInManager<User> signInManager;
        //private readonly RoleManager<IdentityRole> roleManager;
        public MembersController(
            TennisTMDbContext dbContext,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager
        ){
            this.dbContext = dbContext;
            this.userManager = userManager;
            //this.signInManager = signInManager;
            //this.roleManager = roleManager; 
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var users = await dbContext.Users.ToListAsync();
            var coaches = await dbContext.Coaches.ToListAsync();
            var members = users.Where(user => !coaches.Any(coach=> coach.UserId == user.Id)).ToList();
            return View(members);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Promote(string Id)
        {
            var member = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == Id);
            var oldCoach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.UserId == Id);
            if (member != null && oldCoach == null)
            {
                var coach = new Coach()
                {
                    Id = Guid.NewGuid(),
                    UserId = member.Id
                };
                await dbContext.Coaches.AddAsync(coach);
                await dbContext.SaveChangesAsync();
                await userManager.AddToRoleAsync(member, "Coach");

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Coaches()
        {
            var coaches = await dbContext.Coaches.Include(c => c.User).ToListAsync();
            return View(coaches);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Demote(Guid Id)
        {
            var coach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.Id == Id);
            if (coach != null)
            {
                var member = await dbContext.Users.FirstAsync(x => x.Id == coach.UserId);

                dbContext.Coaches.Remove(coach);
                await dbContext.SaveChangesAsync();
                await userManager.RemoveFromRoleAsync(member, "Coach");

                return RedirectToAction("Coaches");
            }
            return RedirectToAction("Coaches");
        }
    }
}
