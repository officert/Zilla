using System.Collections.Generic;
using ZillaMvc.Data;

namespace ZillaMvc.Models
{
    public class HomeModel : LayoutModel
    {
        public IEnumerable<VideoGame> VideoGames { get; set; } 
    }
}