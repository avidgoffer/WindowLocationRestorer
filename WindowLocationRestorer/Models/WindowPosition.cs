using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Diagnostics;
using System.Windows;

namespace WindowLocationRestorer.Models
{
    public class WindowPosition
    {
        public Int32Rect Position { get; set; }
        public string ExecutableName { get; set; }
        public string LastDisplaySeenOn { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public WindowState WindowState { get; set; }

        [JsonIgnore]
        public Process Process { get; set; }

        public override string ToString()
        {
            return Process.MainWindowTitle;
        }
    }
}