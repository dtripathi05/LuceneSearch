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
                if (result.Count != 0)
                {
                    foreach (var result1 in result)
                    {
                        Console.WriteLine($"Name : {result1.Name}\nType : {result1.Type}\nDescription : {result1.Description}");
                    }
                }
                else Console.WriteLine("No Matching Record Found");
                Console.ReadKey();
            }
        }
    }
}
