using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Dapper.Contrib.Extensions;
using K1.Dapper.ContribPlus.Filters;

namespace K1.Dapper.ContribPlus
{
    class Program
    {
        private static readonly IDbConnection dbConnection = new SqlConnection("Server=.;Database=Northwind;User Id=sa;Password=tntseries");

        public Program()
        {
            
        }

        static void Main(string[] args)
        {
            var filters = new Filter[]
            {
                Filter.InClouse("ProductID", 1,2,3,4,5,6,7),
                Filter.Contains("ProductName", "h")
            };
            var products = dbConnection.GetPagedList<Product>
                (offset: 0, fetch: 10, filters: filters);

            var filters1 = new Filter[]
            {
                Filter.Between("CategoryID",1,5)
            };

            var products1 = dbConnection.GetPagedList<Product>(filters: filters1);

            Console.WriteLine("Hello World!");
        }
    }
}
