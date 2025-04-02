using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CS3750Assignment1.Data;
using CS3750Assignment1.Models;
using static CS3750Assignment1.Pages.Assignments.IndexModel;
using static CS3750Assignment1.Pages.Registrations.IndexModel;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore.Internal;

namespace CS3750Assignment1.Pages.Assignments
{
    public class IndexModel : PageModel
    {
        private readonly CS3750Assignment1Context _context;

        public IndexModel(CS3750Assignment1Context context)
        {
            _context = context;
        }

        public IList<Assignment> Assignments { get; set; } = default!;
        public IList<AssignmentViewModel> SubmittedAssignments { get; set; } = new List<AssignmentViewModel>();

        public AssignmentData[] SubmittedAssignmentData { get; set; }

        public int CourseID { get; private set; }
        public bool IsInstructor { get; private set; }
        public HashSet<int> submissions { get; private set; } = new HashSet<int>();

        [BindProperty]
        public int studentFinalGrade { get; set; }

        [BindProperty]
        public int possibleTotalGrade { get; set; }

        [BindProperty]
        public string letterGrade { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            studentFinalGrade = 0;
            possibleTotalGrade = 0;
            letterGrade = string.Empty;

            if (!int.TryParse(Request.Cookies["LoggedUserID"], out int userId))
            {
                return RedirectToPage("/Index");
            }

            string userRole = Request.Cookies["LoggedUserRole"];
            IsInstructor = userRole == "Instructor";

            if (id != null)
            {
                CourseID = id.Value;
            }
            else if (Request.Cookies.ContainsKey("SelectedCourse"))
            {
                CourseID = int.Parse(Request.Cookies["SelectedCourse"]);
            }
            else
            {
                return RedirectToPage("/WelcomeStudent");
            }

            Response.Cookies.Append("SelectedCourse", CourseID.ToString(), new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTime.Now.AddMinutes(180)
            });

            // Get list of assignments.
            var assignmentList = from a in _context.Assignment select a;
            assignmentList = assignmentList.Where(a => a.CourseID == CourseID);

            // Check if student
            if (!IsInstructor)
            {
                // Get list of submissions
                // LINQ JOIN
                var submissionList = (from a in _context.Assignment
                                      join sub in _context.Submission
                                      on a.Id equals sub.AssignmentID
                                      where sub.StudentID == userId
                                      select new AssignmentViewModel
                                      {
                                          Id = sub.Id,
                                          StudentID = sub.StudentID,
                                          CourseID = a.CourseID,
                                          AssignmentID = a.Id,
                                          Title = a.Title,
                                          Description = a.Description,
                                          EarnedPoints = sub.PointsEarned,
                                          MaxPoints = a.MaxPoints,
                                          SubmittedAt = sub.SubmittedAt
                                      });

                submissionList = submissionList.Where(s => s.StudentID == userId);
                
                submissions = _context.Submission.Where(s => s.StudentID == userId)
                .Select(s => s.AssignmentID).ToHashSet();


                // Remove submissions from assignment list. Make Submissions their own list.
                assignmentList = assignmentList.Where(s => !submissions.Contains(s.Id));
                submissionList = submissionList.Where(s => (submissions.Contains(s.AssignmentID) && s.CourseID == CourseID));

                // Gather graded scores
                foreach(var s in submissionList)
                {
                    if (s.EarnedPoints != null)
                    {
                        studentFinalGrade += s.EarnedPoints ?? default(int);
                        possibleTotalGrade += s.MaxPoints;
                    }
                }

                if (possibleTotalGrade > 0)
                {
                    float pointPercent = studentFinalGrade / possibleTotalGrade;
                    if (pointPercent >= 1)
                        letterGrade = "A+";
                    else if (pointPercent >= 0.94)
                        letterGrade = "A";
                    else if (pointPercent >= 0.9)
                        letterGrade = "A-";
                    else if (pointPercent >= 0.87)
                        letterGrade = "B+";
                    else if (pointPercent >= 0.84)
                        letterGrade = "B";
                    else if (pointPercent >= 0.8)
                        letterGrade = "B-";
                    else if (pointPercent >= 0.77)
                        letterGrade = "C+";
                    else if (pointPercent >= 0.74)
                        letterGrade = "C";
                    else if (pointPercent >= 0.7)
                        letterGrade = "C-";
                    else if (pointPercent >= 0.67)
                        letterGrade = "D+";
                    else if (pointPercent >= 0.64)
                        letterGrade = "D";
                    else
                        letterGrade = "F";
                }
                else
                    letterGrade = "NA";

                // Finalize view data.
                SubmittedAssignments = await submissionList.ToListAsync();

                SubmittedAssignmentData = new AssignmentData[SubmittedAssignments.Count()];
                int currentIndex = 0;

                //go through the submission list and get the low, high, and mean scores for those assignments
                foreach(var submission in submissionList)
                {
                    List<int> submissionScores = new List<int>();
                    var currAssignList = await _context.Submission.Where(s => s.AssignmentID == submission.AssignmentID).ToListAsync();
                    //fancy lambda expression to add not null submission scores to the submissionScores list for grabbing data
                    //this way, only assignments with at least one graded submission are used
                    currAssignList.ForEach(sub => 
                    {
                        if (sub.PointsEarned is not null)
                        {
                            submissionScores.Add((int)sub.PointsEarned);
                        }
                    });
                    int assignMax = 0;
                    int assignMin = int.MaxValue;
                    double assignMean = 0.00;

                    if (submissionScores.Count > 0)
                    {
                        for (int i = 0; i < submissionScores.Count; i++)
                        {
                            if (submissionScores[i] > assignMax)
                            {
                                assignMax = submissionScores[i];
                            }
                            if (submissionScores[i] < assignMin)
                            {
                                assignMin = submissionScores[i];
                            }
                            assignMean += submissionScores[i];
                        }

                        assignMean /= submissionScores.Count;

                        if (submission.EarnedPoints is not null)
                        {
                            SubmittedAssignmentData[currentIndex] = new AssignmentData(submission.AssignmentID, assignMin, assignMax, assignMean, (int) submission.EarnedPoints, submission.Title);
                        }
                        else
                        {
                            SubmittedAssignmentData[currentIndex] = new AssignmentData(submission.AssignmentID, assignMin, assignMax, assignMean, 0, submission.Title);
                        }
                        currentIndex++;

                    }

                    //operate on the newly created list to grab the desired data, then add it to the array
                }

                // GET INFORMATION FOR GRAPH VISUALIZATION

                //iterate through the submission list and grab the following information for each assignment:
                // the low score in the class
                // the high score in the class
                // the average score in the class
                // this student's score (given in the submissionList

                //END GRAPH INFORMATION
            }

            // Finalize view data.
            Assignments = await assignmentList.ToListAsync();

            return Page();
        }


    }

    // Custom ViewModel to hold Registration and Course details
    public class AssignmentViewModel
    {
        // New fields for Course information
        public int Id { get; set; }
        public int StudentID { get; set; }
        public int CourseID { get; set; }
        public int AssignmentID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? EarnedPoints { get; set; }
        public int MaxPoints { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    /// <summary>
    /// Custom Object for holding assignment data
    /// </summary>
    public class AssignmentData
    {
        public int Id { get; set; }
        public int AssignLow { get; set; }
        public int AssignHigh { get; set; }
        public double AssignMean { get; set; }
        public int AssignPoints { get; set; }
        public string AssignTitle { get; set; }

        public AssignmentData(int assignID, int assignLowScore, int assignHighScore, double assignMeanScore, int assignPoints, string assignTitle)
        {
            Id = assignID;
            AssignLow = assignLowScore;
            AssignHigh = assignHighScore;
            AssignMean = assignMeanScore;
            AssignPoints = assignPoints;
            AssignTitle = assignTitle;

        }
    }
}
