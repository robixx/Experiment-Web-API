using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.RequestModels
{
    public class SocialMediaRequest
    {
       
        public string name { get; set; } = string.Empty;

        public string url { get; set; } = string.Empty;

        public bool status { get; set; }
    }
}
