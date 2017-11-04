using System;
using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Data
{
    public class Homework
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ContentType Type { get; set; }

        public DateTime SubmissionDate { get; set; }

        public int CourseId { get; set; }

        public Course Course { get; set; }

        public int StudentId { get; set; }

        public Student Student { get; set; }
    }
}
