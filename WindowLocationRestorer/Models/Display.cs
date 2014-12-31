using Newtonsoft.Json;
using System.Windows;

namespace WindowLocationRestorer.Models
{
    public class Display
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string GdiDeviceName { get; set; }
        public uint DisplayId { get; set; }
        public Point Position { get; set; }
        [JsonIgnore]
        public bool Active { get; set; }
    }
}