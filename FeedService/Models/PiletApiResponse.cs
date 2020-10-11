using System;
using System.Collections.Generic;
using System.Text;

namespace FeedService.Models
{
    public class PiletApiResponse
    {
        public PiletApiResponse()
        {
            Items = new List<PiletMetaData>();
        }
        public List<PiletMetaData> Items { get; set; }
    }

    public class PiletMetaData
    {
        public string Name { get; set; }
        public string Version { get; set; }
        public string Link { get; set; }
        public string RequireRef { get; set; }
        public string Integrity { get; set; }
        public string Custom { get; set; }
    }
}
