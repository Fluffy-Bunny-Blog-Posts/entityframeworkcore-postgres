namespace WebApp.Models
{
    public class City : BaseEntity
    {
        public string StateFK { get; set; }
        public string CountyFK { get; set; }
        public string Name { get; set; }
    }
}
