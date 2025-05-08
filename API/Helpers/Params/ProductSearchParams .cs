namespace API.Helpers.Params
{
    public class ProductSearchParams : PaginationParams
    {
        private string _keyword = string.Empty;
        public string Keyword
        {
            get => _keyword;
            set => _keyword = value.ToUpper();
        }
    }
}
