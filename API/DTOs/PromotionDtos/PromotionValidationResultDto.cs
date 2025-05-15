using API.Data.Entities;
using AutoMapper.Configuration.Annotations;
using System.Text.Json.Serialization;

namespace API.DTOs.PromotionDtos
{
    public class PromotionValidationResultDto<T>
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public T? Promotion { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public decimal? DiscountAmount { get; set; }
    }
}
