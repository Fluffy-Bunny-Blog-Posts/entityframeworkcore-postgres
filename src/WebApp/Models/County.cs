namespace WebApp.Models
{
    public class County : BaseEntity
    {
        public string StateFK { get; set; }
        public string Name { get; set; }
    }
}
