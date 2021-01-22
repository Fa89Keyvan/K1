using K1.Dapper.ContribPlus.Filters;
using System.Collections.Generic;

namespace Dapper.Contrib.Extensions
{
    public class PageListRequest
    {
        private List<Filter> _filters;
        public PageListRequest()
        {
            this._filters = new List<Filter>();
            this.Fetch = 1000;
            this.Offset = 0;
            this.OrderDirection = OrderDir.ASC;
            this.WithNolock = false;
        }

        public OrderDir OrderDirection { get; set; }


        private int _offset;
        public int Offset
        {
            get => _offset;
            set => _offset = value < 0 ? 0 : value;
        }

        private int _fetch;
        public int Fetch
        {
            get => _fetch;
            set => _fetch = value < 0 ? 0 : value;
        }

        private string[] _orders;

        public string[] Orders
        {
            get => _orders ?? new string[1] { " 1 " };
            set => _orders = value;
        }

        public bool WithNolock { get; set; }

        public List<Filter> GetFilters() => _filters;

        public PageListRequest AddFilterAddSimple(string name, Operators @operator, object value)
        {
            _filters.Add(new SimpleFilter(name, @operator, value));
            return this;
        }

        public PageListRequest AddFilterIsNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNull));
            return this;
        }

        public PageListRequest AddFilterIsNotNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNotNull));
            return this; 
        }

        public PageListRequest AddFilterInClouse(string name, params object[] values)
        {
            _filters.Add(new InClouseFilter(name, values));
            return this; 
        }

        public PageListRequest AddFilterStartsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.StartsWith, value));
            return this;
        }

        public PageListRequest AddFilterEndsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.EndsWith, value));
            return this;
        }

        public PageListRequest AddFilterContains(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.Contains, value));
            return this;
        }

        public PageListRequest AddFilterBetween(string name, object min, object max)
        {
            _filters.Add(new BetweenFilter(name, min, max));
            return this; 
        }
    }
}
