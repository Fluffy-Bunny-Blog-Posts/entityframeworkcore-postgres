using System;

namespace WebApp.Models
{
    public class BaseEntity
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
