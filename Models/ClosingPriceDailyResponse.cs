using System.Text.Json.Serialization;

namespace StrategyMate.Models
{
    /// <summary>
    /// API response for the daily closing price list.
    /// </summary>
    public class ClosingPriceDailyResponse
    {
        /// <summary>
        /// List of daily closing prices.
        /// </summary>
        [JsonPropertyName("closingPriceDaily")]
        public List<DailyClosingPrice> ClosingPrices { get; set; } = new();
    }
}
