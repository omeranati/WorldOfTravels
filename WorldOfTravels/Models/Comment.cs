using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WorldOfTravels.Models
{
    public class Comment
    {
        public int ID { get; set; }

        [Required]
        public int PostID { get; set; }

        [Required]
        public string Content { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Publish Date")]
        public DateTime CreationDate { get; set; }

        public virtual Post Post { get; set; }

        public string UploaderUsername { get; set; }
    }
}
