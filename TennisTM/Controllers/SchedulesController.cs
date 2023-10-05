using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Microsoft.EntityFrameworkCore;
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
                    EventTime = "",
                    Location = scheduleRes.Location,
                    Coach = tempCoach
                };
                await dbContext.Schedules.AddAsync(schedule);
                await dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            else
            {
                ViewData["message"] = "Item Add Failed.";
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
                    ViewData["message"] = "Item Add Failed.";
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
    }
}
