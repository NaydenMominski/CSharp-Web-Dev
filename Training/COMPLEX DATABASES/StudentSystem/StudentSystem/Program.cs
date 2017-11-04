
namespace StudentSystem
{
    using System;
    using Data;
    using Microsoft.EntityFrameworkCore;

    public class Program
    {
        public static void Main()
        {
            using (var db = new StudentSystemDbContext())
            {
                db.Database.Migrate();


                const int totalStudents = 25;
                const int totalCourses = 10;
                var currentDate = DateTime.Now;


                //Students

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

                for (int i = 0; i < totalCourses; i++)
                {
                    db.Courses.Add(new Course
                    {
                        Name = $"Course {i}",
                        Description = $"Course Details {i}",
                        Price = 100 * i,
                        Startdate = currentDate.AddDays(i),
                        EndDate = currentDate.AddDays(20 + i)
                    });
                }




                db.SaveChanges();
            }
        }
    }
}
