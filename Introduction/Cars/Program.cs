using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cars
{
    class Program
    {
        static void Main(string[] args)
        {
            var cars = ProcessFile("fuel.csv");

            var query =
                from car in cars
                where car.Manufacturer == "BMW" && car.Year == 2016
                orderby car.Combined descending, car.Name ascending
                select new
                {
                    car.Manufacturer,
                    car.Name,
                    car.Combined
                };
            
            var top = cars
                .OrderByDescending(c => c.Combined)
                .ThenBy(c=> c.Name)
                .Select(c => c)
                .First(c => c.Manufacturer == "BMW" && c.Year == 2016);
            Console.WriteLine($"{top.Name}");
             
            var resultAny = cars.Any(c => c.Manufacturer == "Ford");
            var resultAll = cars.All(c => c.Manufacturer == "Ford");
            Console.WriteLine(resultAny);
            Console.WriteLine(resultAll);

            foreach (var car in query.Take(10))
            {
                Console.WriteLine($"{car.Name} : {car.Combined}");
            }
        }

        private static IEnumerable<Car> ProcessFile(string path)
        {
            return File.ReadAllLines(path)
                .Skip(1)
                .Where(line => line.Length > 1)
                .ToCar();
                //from line in File.ReadAllLines(path).Skip(1)
                //where line.Length > 1
                //select Car.ParseFromCsv(line);
        }

    }

    public static class CarExtensions
    {
        public static IEnumerable<Car> ToCar(this IEnumerable<string> source)
        {
            foreach (var line in source)
            {
                var columns = line.Split(',');
                yield return new Car
                {
                    Year = int.Parse(columns[0]),
                    Manufacturer = columns[1],
                    Name = columns[2],
                    Displacement = double.Parse(columns[3]),
                    Cylinders = int.Parse(columns[4]),
                    City = int.Parse(columns[5]),
                    Highway = int.Parse(columns[6]),
                    Combined = int.Parse(columns[7])
                };
            }
        }
    }
}
