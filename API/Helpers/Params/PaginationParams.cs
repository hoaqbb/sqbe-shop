namespace API.Helpers.Params
{
    public class PaginationParams
    {
        private int _index = 1;
        // set pageIndex = 1 if the value set < 0
        public int PageIndex
        {
            get => _index;
            set => _index = (value < 1) ? _index : value;
        }

        private const int MaxPageSize = 16;

        private int _pageSize = 8;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
