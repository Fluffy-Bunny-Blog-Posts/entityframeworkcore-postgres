using System.Collections.Generic;

namespace WebApp.Services
{
    public class StateModel
    {
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public List<string> Counties { get; set; }
    }
}
