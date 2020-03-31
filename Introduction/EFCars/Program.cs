using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace EFCars
{
    class Program
    {
        static void Main(string[] args)
        {
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<CarDb>());
            InsertData();
            QueryData();
        }

        private static void QueryData()
        {
            var db = new CarDb();
            db.Database.Log = Console.WriteLine;

            var query = db.Cars
                .Where(c => c.Manufacturer == "BMW")
                .OrderByDescending(c => c.Combined)
                .ThenBy(c => c.Name)
                .Take(10);

            foreach (var car in query)
            {
                Console.WriteLine($"{car.Name}: {car.Combined}");
            }

            var query2 =
                db.Cars.GroupBy(c => c.Manufacturer)
                        .Select(g => new
                        {
                            Name = g.Key,
                            Cars = g.OrderByDescending(c => c.Combined).Take(2)
                        });
            //var query2 =
            //    from car in db.Cars
            //    group car by car.Manufacturer into manufacturer
            //    select new
            //    {
            //        Name = manufacturer.Key,
            //        Cars = (from car in manufacturer
            //                orderby car.Combined descending
            //                select car).Take(2)
            //    };

            foreach (var group in query2)
            {
                Console.WriteLine(group.Name);
                foreach (var car in group.Cars)
                {
                    Console.WriteLine($"\t{car.Name}: {car.Combined}");
                }
            }
        }

        private static void InsertData()
        {
            var cars = ProcessCars("fuel.csv");
            var db = new CarDb();

            if (!db.Cars.Any())
            {
                foreach (var car in cars)
                {
                    db.Cars.Add(car);
                }
                db.SaveChanges();
            }
        }

        private static IEnumerable<Car> ProcessCars(string path)
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
