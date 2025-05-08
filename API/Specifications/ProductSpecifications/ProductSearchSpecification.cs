using API.Data.Entities;
using API.Helpers.Params;

namespace API.Specifications.ProductSpecifications
{
    public class ProductSearchSpecification : BaseSpecification<Product>
    {
        public ProductSearchSpecification(ProductSearchParams param)
            : base(x => x.IsVisible == true && x.Name.Contains(param.Keyword))
        {
            ApplyOrderByDescending(x => x.CreateAt);
            ApplyPaging(param.PageSize * (param.PageIndex - 1), param.PageSize);
        }
    }
}
