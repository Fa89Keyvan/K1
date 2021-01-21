using Dapper.Contrib.Extensions;

namespace K1.Dapper.ContribPlus
{
    [Table("Products")]
    class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
    }
}
