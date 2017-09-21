using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LucentDemo;
namespace LuceneSearchingDemoMain
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                List<PointOfInterest> InterestsList = PointOfInternetDataRepository.GetAll();
                LuceneSearch.AddUpdateLuceneIndex(InterestsList);
                Console.WriteLine("\nProvide Your Interest ");
                List<PointOfInterest> result = LuceneSearch.Search(Console.ReadLine()).ToList();
                if (result[0] != null)
                {
                    Console.WriteLine($"Name : {result[0].Name}\nType : {result[0].Type}\nDescription : {result[0].Description}");
                }
                else Console.WriteLine("No Matching Record Found");
                Console.ReadKey();
            }
        }
    }
}
