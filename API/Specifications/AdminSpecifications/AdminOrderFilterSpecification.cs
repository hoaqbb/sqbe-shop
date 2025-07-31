using API.Data.Entities;
using API.Data.Enums;
using API.Helpers.Params;

namespace API.Specifications.AdminSpecifications
{
    public class AdminOrderFilterSpecification : BaseSpecification<Order>
    {
        public AdminOrderFilterSpecification(AdminOrderFilterParams param)
            : base(
            x => (
            (param.AmountFrom == null || x.Amount >= param.AmountFrom)
            && (param.AmountTo == null || x.Amount <= param.AmountTo)
            && (param.Status == null || x.Status == param.Status)
            && (param.isDiscounted == null || (param.isDiscounted == true ? x.DiscountAmount > 0 : x.DiscountAmount == 0))
            && (param.PaymentMethod == null || x.Payment.Provider == param.PaymentMethod)
            ))
        {
            ApplyPaging(param.PageSize * (param.PageIndex - 1), param.PageSize);

            switch (param.Sort)
            {
                case "dateAsc":
                    ApplyOrderBy(x => x.CreateAt);
                    break;
                case "amountAsc":
                    ApplyOrderBy(x => x.Amount);
                    break;
                case "amountDesc":
                    ApplyOrderByDescending(x => x.Amount);
                    break;
                default:
                    ApplyOrderByDescending(x => x.CreateAt);
                    break;
            }
        }
    }
}
