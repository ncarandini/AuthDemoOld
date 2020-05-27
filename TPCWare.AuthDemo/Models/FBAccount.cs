// FBAccount.cs /Users/biagioparuolo/Projects/AuthDemo/TPCWare.AuthDemo/Models
// Biagio Paruolo
// 20205272145
using System;

using Newtonsoft.Json;

namespace TPCWare.AuthDemo.Models
{
    public class FBAccount
    {


        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Locale { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        [JsonProperty("picture")]
        public Picture Picture { get; set; }

    }
}
