using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class County : BaseEntity
    {
        public string StateFK { get; set; }
        public string Name { get; set; }
        [ForeignKey("CountyFK")]
        public virtual ICollection<City> Cities { get; set; }

    }
}
