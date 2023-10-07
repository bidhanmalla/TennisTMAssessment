using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TennisTM.Data;
using TennisTM.Models;

namespace TennisTM.Controllers
{
    [Authorize]
    public class SchedulesController : Controller
    {
        private readonly TennisTMDbContext dbContext;
        public SchedulesController(TennisTMDbContext dbContext)
        {
            this.dbContext = dbContext;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var schedules = await dbContext.Schedules.Include(x => x.Coach).ThenInclude(x => x.User).ToListAsync();
            return View(schedules);
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add()
        {
            var coaches = await dbContext.Coaches.Include(c => c.User).ToListAsync();
            var tempSchedule = new TempSchedule();
            foreach (var coach in coaches)
            {
                tempSchedule.CoachList.Add(new SelectListItem
                {
                    Text = coach.User.Name,
                    Value = Convert.ToString(coach.Id)
                });
            };
            return View(tempSchedule);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Add(TempSchedule scheduleRes)
        {
            var coaches = await dbContext.Coaches.Include(c => c.User).ToListAsync();
            var tempCoach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.Id == scheduleRes.CoachId);
            if (tempCoach != null)
            {
                var schedule = new Schedule()
                {
                    Id = Guid.NewGuid(),
                    EventName = scheduleRes.EventName,
                    Location = scheduleRes.Location,
                    EventTime = scheduleRes.EventTime,
                    Coach = tempCoach
                };
                await dbContext.Schedules.AddAsync(schedule);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["message"] = "Adding Schedule Failed.";
                foreach (var coach in coaches)
                {
                    scheduleRes.CoachList.Add(new SelectListItem
                    {
                        Text = coach.User.Name,
                        Value = Convert.ToString(coach.Id)
                    });
                };
                return View(scheduleRes);
            };
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(Guid Id) 
        {
            var coaches = await dbContext.Coaches.Include(c => c.User).ToListAsync();
            var schedule = await dbContext.Schedules.Include(x => x.Coach).FirstOrDefaultAsync(x => x.Id == Id);
            if (schedule != null)
            {
                var tempSchedule = new TempSchedule()
                {
                    Id = schedule.Id,
                    EventName = schedule.EventName,
                    EventTime = schedule.EventTime,
                    Location = schedule.Location,
                    CoachId = schedule.Coach.Id,
                };
                foreach (var coach in coaches)
                {
                    tempSchedule.CoachList.Add(new SelectListItem
                    {
                        Text = coach.User.Name,
                        Value = Convert.ToString(coach.Id)
                    });
                };
                return View(tempSchedule);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(TempSchedule scheduleRes)
        {
            var schedule = await dbContext.Schedules.FirstOrDefaultAsync(x => x.Id == scheduleRes.Id);
            var coaches = await dbContext.Coaches.Include(c => c.User).ToListAsync();
            var tempCoach = await dbContext.Coaches.FirstOrDefaultAsync(x => x.Id == scheduleRes.CoachId);
            if (schedule != null)
            {
                schedule.EventName = scheduleRes.EventName;
                schedule.EventTime = scheduleRes.EventTime;
                schedule.Location = scheduleRes.Location;
                if (tempCoach != null) schedule.Coach = tempCoach;
                else
                {
                    ViewData["message"] = "Editing Schedule Failed.";
                    foreach (var coach in coaches)
                    {
                        scheduleRes.CoachList.Add(new SelectListItem
                        {
                            Text = coach.User.Name,
                            Value = Convert.ToString(coach.Id)
                        });
                    };
                    return View(scheduleRes);
                }
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(scheduleRes);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid Id)
        {
            var schedule = await dbContext.Schedules.FirstOrDefaultAsync(x => x.Id == Id);
            if (schedule != null )
            {
                dbContext.Schedules.Remove(schedule);
                await dbContext.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Join(Guid Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.Include(x => x.Schedules).FirstOrDefaultAsync(x => x.Id == userId);
            var schedule = await dbContext.Schedules.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == Id);
            if (user != null && schedule != null)
            {
                schedule.Users.Add(user);
                user.Schedules.Add(schedule);
                await dbContext.SaveChangesAsync();
            };
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Leave(Guid Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.Include(x => x.Schedules).FirstOrDefaultAsync(x => x.Id == userId);
            var schedule = await dbContext.Schedules.Include(x => x.Users).FirstOrDefaultAsync(x => x.Id == Id);
            if (user != null && schedule != null)
            {
                schedule.Users.Remove(user);
                user.Schedules.Remove(schedule);
                await dbContext.SaveChangesAsync();
            };
            return RedirectToAction("Joined");
        }
        [HttpGet]
        public async Task<IActionResult> Joined()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await dbContext.Users.Include(x => x.Schedules)
                .ThenInclude(y => y.Coach)
                .ThenInclude(z => z.User)
                .FirstOrDefaultAsync(x => x.Id == userId);
            if (user != null)
            {
                return View(user.Schedules.ToList());
            }
            return RedirectToAction("Index");
        }
    }
}
