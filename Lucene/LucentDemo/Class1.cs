using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene;
using System.Net.Http;

namespace LucentDemo
{
    public class PointOfInterest
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
    }
    public static class PointOfInternetDataRepository
    {
        public static PointOfInterest Get(string name)
        {
            return GetAll().SingleOrDefault(x => x.Name.Equals(name));
        }
        public static List<PointOfInterest> GetAll()
        {
            return new List<PointOfInterest> {
        new PointOfInterest { Name = "VivantaTaj",Type="Hotel", Description = "5 Star Hotel Chain In India"},
        new PointOfInterest { Name = "Kingfisher",Type="Flight", Description = "Airlines Provider"},
        new PointOfInterest { Name = "Chicago",Type="City", Description = "City in USA"},
        new PointOfInterest { Name = "Mumbai",Type="City", Description = "City In India Where Dreams Come True"},
        new PointOfInterest { Name = "Hayat",Type="Hotel", Description = "One Of The Renewed Hotel Of Pune City "},
    };
        }
    }
}

