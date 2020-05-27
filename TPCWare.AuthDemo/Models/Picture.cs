// Picture.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Models
// Biagio Paruolo
// 20205272258
using System;

using Newtonsoft.Json;
namespace TPCWare.AuthDemo.Models
{
    public class Data
    {
        public int height { get; set; }
        public string is_silhouette { get; set; }
        public string url { get; set; }
        public int width { get; set; }
    }

    public class Picture
    {
        [JsonProperty("data")]
        public Data data { get; set; }
    }
}
