using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Models
{
    public class State : BaseEntity
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        [ForeignKey("StateFK")]
        public virtual ICollection<County> Counties { get; set; }
    }
}
