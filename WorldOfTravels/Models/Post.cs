using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorldOfTravels.Models
{
    public class Post
    {
        public int ID { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        [Display(Name = "Publish Date")]
        [DataType(DataType.Date)]
        public DateTime PublishDate { get; set; }

        public virtual int CountryID { get; set; }
        public virtual Country Country { get; set; }
    }
}
