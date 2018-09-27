using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WorldOfTravels.Models
{
    public class Country
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Continent Continent { get; set; }

        [DisplayName("Is Tropical?")]
        public bool IsTropical { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }

    public enum Continent
    {
        America,
        Asia,
        Africa,
        Europa,
        Australia
    }
}

