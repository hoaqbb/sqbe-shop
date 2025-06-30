using API.Data.Entities;
using API.DTOs.PaymentDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class PaymentProfiles : Profile
    {
        public PaymentProfiles()
        {
            CreateMap<Payment, PaymentDto>();
        }
    }
}
