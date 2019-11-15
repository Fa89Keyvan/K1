using System;

namespace K1.MyConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetEmployee());

            Console.ReadLine();
        }



        static Person GetPerson()
        {
            return new Person { ID = 1, FirstName = "Ali" };
        }

        static Employee GetEmployee()
        {
            var emp = (Employee)GetPerson();
            emp.Salary = 200;
            return emp;
        }

    }

    public class Person
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
    }

    public class Employee : Person
    {
        public long Salary { get; set; }

        public override string ToString()
        {
            return $"ID: {ID}, FirstName: {FirstName}, Salary: {Salary}";
        }
    }
}
