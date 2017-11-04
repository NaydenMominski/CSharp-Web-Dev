
namespace StudentSystem
{
    using System;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Collections.Generic;

    public class Program
    {
        public static void Main()
        {
            using (var db = new StudentSystemDbContext())
            {
                db.Database.Migrate();

                //seedData(db);
                //PrintStudentsWithHomeworks(db);
                //PrintCoursesAndResourses(db);
                //PrintCoursesWithMoreThanFiveResouces(db);
                //PrintCourseActionOnDate(db);
                PrintStudentsWithPriceCourse(db);
            }
        }



        private static void seedData(StudentSystemDbContext db)
        {
            const int totalStudents = 25;
            const int totalCourses = 10;
            var currentDate = DateTime.Now;

            //Students
            Console.WriteLine("Adding Students");

            for (int i = 0; i < totalStudents; i++)
            {
                db.Students.Add(new Student
                {
                    Name = $"Student {i}",
                    RegistrationDate = currentDate.AddDays(i),
                    Birthday = currentDate.AddYears(-20).AddDays(i),
                    Phone = $" Random Phone {i}"

                });

            }

            //Courses
            var addedCourses = new List<Course>();

            for (int i = 0; i < totalCourses; i++)
            {
                var courses = new Course
                {
                    Name = $"Course {i}",
                    Description = $"Course Details {i}",
                    Price = 100 * i,
                    Startdate = currentDate.AddDays(i),
                    EndDate = currentDate.AddDays(20 + i)
                };

                addedCourses.Add(courses);

                db.Courses.Add(courses);
            }

            db.SaveChanges();

            var random = new Random();

            //Student in Courses

            var studentIds = db
                .Students
                .Select(s => s.Id)
                .ToList();

            for (int i = 0; i < totalCourses; i++)
            {
                var currentCourse = addedCourses[i];
                var studentInCourses = random.Next(2, totalStudents / 2);

                for (int j = 0; j < studentInCourses; j++)
                {
                    var studentId = studentIds[random.Next(0, studentIds.Count)];

                    if (!currentCourse.Students.Any(s => s.StudentId == studentId))
                    {
                        currentCourse.Students.Add(new StudentCourse
                        {
                            StudentId = studentId
                        });
                    }
                    else
                    {
                        j--;
                    }
                    
                }

                var resoursesInCourse = random.Next(2, 20);
                var types = new[] { 0, 1, 2, 999 };

                for (int j = 0; j < resoursesInCourse; j++)
                {
                    currentCourse.Resourses.Add(new Resourse
                    {
                        Name = $"Resourse {i} {j}",
                        Url = $"URL {i} {j}",
                        Type = (ResourseType)types[random.Next(0, types.Length)]
                    });

                }


            }

            db.SaveChanges();

            //Homeworks

            for (int i = 0; i < totalCourses; i++)
            {
                var currentCourse = addedCourses[i];

                var studentsInCourseIds = currentCourse
                    .Students
                    .Select(s => s.StudentId)
                    .ToList();

                for (int j = 0; j < studentsInCourseIds.Count; j++)
                {
                    var totalHomeworks = random.Next(2, 5);

                    for (int k = 0; k < totalHomeworks; k++)
                    {


                        db.Homeworks.Add(new Homework
                        {
                            Content = $"Content Homework {i}",
                            SubmissionDate = currentDate.AddDays(-1),
                            Type = ContentType.Zip,
                            StudentId = studentsInCourseIds[j],
                            CourseId = currentCourse.Id

                        });
                    }
                }
                db.SaveChanges();

            }

        }

        private static void PrintStudentsWithHomeworks(StudentSystemDbContext db)
        {
            var result = db
                .Students
                .Select(s => new
                {
                    s.Name,
                    Homework = s.Homeworks.Select(h => new
                    {
                        h.Content,
                        h.Type
                    })
                }).ToList();

            foreach (var student in result)
            {
                Console.WriteLine(student.Name);
                foreach (var homework in student.Homework)
                {
                    Console.WriteLine($"---{homework.Content}-{homework.Type}");
                }
            }
        }

        private static void PrintCoursesAndResourses(StudentSystemDbContext db)
        {
            var result = db
                .Courses
                .OrderBy(c => c.Startdate)
                .ThenBy(c => c.EndDate)
                .Select(c => new
                {
                    c.Name,
                    c.Description,
                    Resourse = c.Resourses.Select(r => new
                    {
                        r.Name,
                        r.Url,
                        r.Type
                    })
                })
                .ToList();

            foreach (var course in result)
            {
                Console.WriteLine($"{course.Name}- {course.Description}");

                foreach (var resourse in course.Resourse)
                {
                    Console.WriteLine($"{resourse.Name}-{resourse.Type}- {resourse.Url}");
                }
            }
        }

        private static void PrintCoursesWithMoreThanFiveResouces(StudentSystemDbContext db)
        {
            var result = db
                 .Courses
                 .Where(c => c.Resourses.Count > 5)
                 .OrderByDescending(c => c.Resourses.Count)
                 .ThenByDescending(c => c.Startdate)
                 .Select(c => new
                 {
                     c.Name,
                     Resourse = c.Resourses.Count

                 })
                 .ToList();

            foreach (var course in result)
            {
                Console.WriteLine($"{course.Name} -{course.Resourse}");
            }
        }

        private static void PrintCourseActionOnDate(StudentSystemDbContext db)
        {
            var date = DateTime.Now.AddDays(25);

            var result = db
                .Courses
                .Where(c => c.Startdate < date && date > c.EndDate)
                .Select(c => new
                {
                    c.Name,
                    c.Startdate,
                    c.EndDate,
                    Duration = c.EndDate.Subtract(c.Startdate),
                    Studens = c.Students.Count
                })
                .OrderByDescending(c => c.Studens)
                .ThenByDescending(c => c.Duration)
                .ToList();

            foreach (var course in result)  
            {
                Console.WriteLine($"{course.Name}- {course.Startdate.ToShortDateString()}- { course.EndDate.ToShortDateString()}");
                Console.WriteLine($"--------Duration:{course.Duration.TotalDays}");
                Console.WriteLine($"---Students: {course.Studens}");
                Console.WriteLine("---------------");
            }
        }


        private static void PrintStudentsWithPriceCourse(StudentSystemDbContext db)
        {
            var result = db
                .Students
                .Where(s => s.Courses.Any())
                .Select(s => new
                {
                    s.Name,
                    Couses = s.Courses.Count,
                    TotalPrice = s.Courses.Sum(c => c.Course.Price),
                    AveragePrice = s.Courses.Average(c => c.Course.Price)
                })
                .OrderByDescending(c => c.TotalPrice)
                .ThenByDescending(c => c.Couses)
                .ThenBy(c => c.Name)
                .ToList();

            foreach (var student in result)
            {
                Console.WriteLine($"{student.Name}- {student.Couses} - {student.TotalPrice} - {student.AveragePrice}");
            }
        }

    }
}
