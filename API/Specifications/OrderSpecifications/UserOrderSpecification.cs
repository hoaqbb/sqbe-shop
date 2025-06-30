using API.Data.Entities;
using API.Helpers.Params;

namespace API.Specifications.OrderSpecifications
{
    public class UserOrderSpecification : BaseSpecification<Order>
    {
        public UserOrderSpecification(UserOrderParams param, Guid userId) 
            : base(x => x.UserId == userId)
        {
            ApplyOrderByDescending(o => o.CreateAt);
            ApplyPaging(param.PageSize * (param.PageIndex - 1), param.PageSize);
        }
    }
}
