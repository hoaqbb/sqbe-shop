namespace API.Helpers
{
    public static class CurrencyHelper
    {
        private const decimal VNDtoUSDExchangeRate = 25900;

        public static decimal VndToUsd(decimal vnd)
        {
            return Math.Round(vnd / VNDtoUSDExchangeRate, 2);
        }
    }
}
