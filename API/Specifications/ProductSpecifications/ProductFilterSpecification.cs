using API.Data.Entities;
using API.Helpers.Params;

namespace API.Specifications.ProductSpecifications
{
    public class ProductFilterSpecification : BaseSpecification<Product>
    {
        public ProductFilterSpecification(ProductFilterParams param)
            : base(x => x.IsVisible == true
            && (string.IsNullOrEmpty(param.Category) ||
                (param.Category.ToLower() == "sale"
                    ? x.Discount > 0
                    : x.Category != null && x.Category.Slug.ToLower() == param.Category.ToLower())
            )
            && (param.Colors == null || x.ProductColors.Any(pc => param.Colors.Contains(pc.ColorId)))
            && (param.Sizes == null || x.ProductVariants.Any(pc => param.Sizes.Contains(pc.SizeId)))
            && (param.PriceFrom == null || x.Price >= param.PriceFrom)
            && (param.PriceTo == null || x.Price <= param.PriceTo)
            && (!param.Promotion || x.Discount > 0)
            )
        {
            ApplyPaging(param.PageSize * (param.PageIndex - 1), param.PageSize);

            switch (param.Sort)
            {
                case "dateAsc":
                    ApplyOrderBy(x => x.CreateAt);
                    break;
                case "priceAsc":
                    ApplyOrderBy(x => x.Price);
                    break;
                case "priceDesc":
                    ApplyOrderByDescending(x => x.Price);
                    break;
                default:
                    ApplyOrderByDescending(x => x.CreateAt);
                    break;
            }
        }
    }
}
