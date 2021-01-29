using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Dapper.Contrib.Extensions;
using K1.Dapper.ContribPlus.Filters;
using System.Threading.Tasks;
using System.Diagnostics;

namespace K1.Dapper.ContribPlus
{
    class Program
    {
        //private static readonly IDbConnection dbConnection = new SqlConnection("Server=.;Database=Northwind;User Id=sa;Password=tntseries");
        private static IDbConnection dbConnection => new SqlConnection("Server=.;Database=Northwind;User Id=sa;Password=tntseries");
        public Program()
        {

        }


        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            

            var listRequest = new PageListRequest<Product>();

            listRequest.AddFilterInClouse("ProductID", 1, 2, 3, 4, 5, 6, 7).AddFilterContains("ProductName", "h");

            var watch = new Stopwatch();
            watch.Start();


            Parallel.For(0, 1000, new ParallelOptions { MaxDegreeOfParallelism = 8 }, (i) =>
            {
                dbConnection.GetPagedListAsync<Product>(listRequest).Wait();
            });



            watch.Stop();
            Console.WriteLine(watch.ElapsedMilliseconds + " Ticks: " + watch.ElapsedTicks);
            Console.ReadLine();
        }
    }
}
