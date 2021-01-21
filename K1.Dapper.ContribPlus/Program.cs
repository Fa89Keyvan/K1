using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Dapper.Contrib.Extensions;

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
                Filter.Create("ProductName", Operator.Contains, "a"), 
                Filter.Create("ProductID", Operator.Grather, 5)
            };
            var products = dbConnection.GetPagedList<Product>
                (offset: 5, fetch: 10, filters: filters);

            Console.WriteLine("Hello World!");
        }
    }
}
