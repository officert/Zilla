using System.Collections.Generic;
using ZillaMvc.Data;

namespace ZillaMvc.Models
{
    public class ConsoleModel : LayoutModel
    {
        public Console Console { get; set; }
        public IEnumerable<VideoGame> Games { get; set; }
    }
}