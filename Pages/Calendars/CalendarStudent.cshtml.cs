﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using System.Diagnostics;

namespace CS3750Assignment1.Pages.Calendars
{
    public class CalendarStudentModel : PageModel
    {
        private readonly CS3750Assignment1.Data.CS3750Assignment1Context _context;

        public bool? pageRole = false;


        public CalendarStudentModel(CS3750Assignment1.Data.CS3750Assignment1Context context)
        {
            _context = context;

        }

        public IList<Registration> Registration { get;set; } = default!;

        public IList<Assignment> Assignment { get; set; } = default!;

        public IList<Classes.RegisteredCourse> RegisteredCourses { get;set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int loggedUserID;
            RegisteredCourses = [];
            if (Request.Cookies["LoggedUserID"] is not null)
            {
                try
                {
                    loggedUserID = int.Parse(Request.Cookies["LoggedUserID"]);
                }
                catch 
                {
                    loggedUserID = 0;
                }
                
            }
            else
            {
                loggedUserID = 0;
            }
            
            try
            {
                Registration = await _context.Registration.Where(r => r.Account.Id == loggedUserID)
               //not going to include the account as a part of this query as the cookie contains the account information .Include(r => r.Account)
               .Include(r => r.Course).ToListAsync();

                //get list of courses for this student
                List<int> courseIds = new List<int>();
                foreach (var registration in Registration) {
                    courseIds.Add(registration.CourseID);
                }

                //get list of assignments for classes this student is registered for
                Assignment = await _context.Assignment.Where(a => courseIds.Contains(a.CourseID))
                .ToListAsync();

            }
            catch
            {
                //if an error occurs, skip assembling the calendar events
                return Page();
            }
            //using this Registration IList will allow me to grab all necessary information for displaying events in the calendar
            //Course.MeetingDays//Course.MeetingTime//Course.Name//Course.CourseNumber
            //going to have to parse the MeetingDays and MeetingTime strings
            //maybe lists can auto parse with something
            
            foreach (var registration in Registration)
            {
                if (registration.Course.MeetingDays is not null && registration.Course.MeetingTime is not null)
                {
                    List<string> classDays = new List<string>();
                    string classStart, classEnd, extra;
                    foreach (string day in registration.Course.MeetingDays.Split(','))
                    {
                        classDays.Add(day);
                        //will need to change this. Full Calendar wants the days of the week in number form: 0 for Sunday, 1 for Monday, 2 for Tuesday, etc
                    }
                    string[] times = registration.Course.MeetingTime.Split("-");

                    Classes.RegisteredCourse tempCourse = new Classes.RegisteredCourse(registration.Course.Name, registration.Course.CourseNumber, classDays, times[0], times[1]);

                    RegisteredCourses.Add(tempCourse);
                    
                }

            }

            return Page();
        }
    }

    

}
