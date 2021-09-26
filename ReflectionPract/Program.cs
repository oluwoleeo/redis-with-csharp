using System;

namespace ReflectionPract
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = "Wole";
            DateTime now = DateTime.Now;
            string timestamp = now.ToString("ddMMyyyyHHmmssffff");
            Console.WriteLine($"Hello {name}. Timestamp is {timestamp}!");
            Console.WriteLine(now);
            bool validDate = DateTime.TryParse("2017-09-03 15:12:25", out DateTime pastDateTime);
            TimeSpan timeSpan = now - pastDateTime;
            Console.WriteLine(validDate);
            Console.WriteLine(pastDateTime);
            Console.WriteLine(timeSpan.TotalHours);
        }
    }
}
