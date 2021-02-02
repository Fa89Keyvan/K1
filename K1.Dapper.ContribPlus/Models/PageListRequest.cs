using K1.Dapper.ContribPlus.Filters;
using System.Collections.Generic;

namespace Dapper.Contrib.Extensions
{
    public class PageListRequest<TEntity>
    {
        private List<Filter> _filters;
        public PageListRequest()
        {
            this._filters = new List<Filter>();
            this._fetchCount = 1000;
            this._offsetCount = 0;
            this._orderDirection = OrderDir.ASC;
            this._withNoLock = false;
        }


        #region ' OrderBy '

        private string[] _orders;
        public string[] Orders => _orders ?? new string[1] { " 1 " };

        private OrderDir _orderDirection;
        public OrderDir OrderDirection => _orderDirection;

        public PageListRequest<TEntity> OrderBy(OrderDir direction, params string[] orders)
        {
            this._orderDirection = direction;
            this._orders = orders;
            
            return this;
        }

        #endregion

        #region ' Offset '

        private int _offsetCount;
        public int OffsetCount => _offsetCount;

        public PageListRequest<TEntity> Offset(int offset)
        {
            _offsetCount = offset < 0 ? 0 : offset;
            return this;
        }

        #endregion

        #region ' Fetch '

        private int _fetchCount;
        public int FetchCount => _fetchCount;

        public PageListRequest<TEntity> Fetch(int fetch)
        {
            _fetchCount = fetch < 0 ? 0 : fetch;
            return this;
        }

        #endregion

        #region ' WithNoLock '

        private bool _withNoLock;
        public bool WithNolock => _withNoLock;
        public PageListRequest<TEntity> WithNoLock()
        {
            this._withNoLock = true;
            return this;
        }

        #endregion

        public List<Filter> GetFilters() => _filters;

        public PageListRequest<TEntity> AddFilterAddSimple(string name, Operators @operator, object value)
        {
            _filters.Add(new SimpleFilter(name, @operator, value));
            return this;
        }

        public PageListRequest<TEntity> AddFilterIsNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNull));
            return this;
        }

        public PageListRequest<TEntity> AddFilterIsNotNull(string name)
        {
            _filters.Add(new IsNullFilter(name, NullCondition.IsNotNull));
            return this; 
        }

        public PageListRequest<TEntity> AddFilterInClouse(string name, params object[] values)
        {
            _filters.Add(new InClouseFilter(name, values));
            return this; 
        }

        public PageListRequest<TEntity> AddFilterStartsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.StartsWith, value));
            return this;
        }

        public PageListRequest<TEntity> AddFilterEndsWith(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.EndsWith, value));
            return this;
        }

        public PageListRequest<TEntity> AddFilterContains(string name, string value)
        {
            _filters.Add(new ContainFilter(name, ContaintTypes.Contains, value));
            return this;
        }

        public PageListRequest<TEntity> AddFilterBetween(string name, object min, object max)
        {
            _filters.Add(new BetweenFilter(name, min, max));
            return this; 
        }
    }
}
