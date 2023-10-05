﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TennisTM.Models;

// Add profile data for application users by adding properties to the TennisTMUser class
public class User : IdentityUser
{
    [Required, MaxLength(20)]
    public string Name { get; set; }
    public ICollection<Schedule> Schedules { get; set; }
}

