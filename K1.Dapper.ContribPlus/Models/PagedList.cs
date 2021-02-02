using System.Collections.Generic;

namespace Dapper.Contrib.Extensions
{
    public class PagedList<TModel> where TModel : class
    {
        public int Total { get; set; }
        public int TotalFiltered { get; set; }
        public List<TModel> Data { get; set; }
    }
}
