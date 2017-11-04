
namespace StudentSystem.Data
{
    using System.ComponentModel.DataAnnotations;

    public class Resourse
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ResourseType Type { get; set; }

        [Required]
        public string  Url { get; set; }

        public int CousesId { get; set; }

        public Course Course { get; set; }




    }
}
