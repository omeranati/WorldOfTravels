using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WorldOfTravels.Models
{
    public class User
    {
        [Required]
        public string ID { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public bool IsAdmin { get; set; } 
    }
}