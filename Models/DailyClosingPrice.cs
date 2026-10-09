using System.Text.Json.Serialization;
using StrategyMate.Helpers;

namespace StrategyMate.Models
{
    /// <summary>
    /// Daily closing price data for a single trading instrument (symbol).
    /// </summary>
    public class DailyClosingPrice
    {
        /// <summary>
        /// Unique identifier of the instrument (InsCode).
        /// </summary>
        [JsonPropertyName("insCode")]
        public string InstrumentCode { get; set; } = string.Empty;

        /// <summary>
        /// Gregorian date in yyyyMMdd format (e.g., 20260912).
        /// </summary>
        [JsonPropertyName("dEven")]
        public int DateGregorian { get; set; }

        /// <summary>
        /// Time in HHmmss format (e.g., 122925 means 12:29:25).
        /// </summary>
        [JsonPropertyName("hEven")]
        public int TimeValue { get; set; }

        /// <summary>
        /// Closing price (in Rials).
        /// </summary>
        [JsonPropertyName("pClosing")]
        public decimal ClosingPrice { get; set; }

        /// <summary>
        /// Adjusted closing price (in Rials).
        /// </summary>
        [JsonPropertyName("pDrCotVal")]
        public decimal AdjustedClosingPrice { get; set; }

        /// <summary>
        /// Indicates whether this record is the last trade of the day.
        /// </summary>
        [JsonPropertyName("last")]
        public bool? IsLast { get; set; }

        /// <summary>
        /// First trade price of the day (in Rials).
        /// </summary>
        [JsonPropertyName("priceFirst")]
        public decimal FirstPrice { get; set; }

        /// <summary>
        /// Yesterday's closing price (in Rials).
        /// </summary>
        [JsonPropertyName("priceYesterday")]
        public decimal YesterdayPrice { get; set; }

        /// <summary>
        /// Lowest price of the day (in Rials).
        /// </summary>
        [JsonPropertyName("priceMin")]
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Highest price of the day (in Rials).
        /// </summary>
        [JsonPropertyName("priceMax")]
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// Price change compared to the previous day (in Rials).
        /// </summary>
        [JsonPropertyName("priceChange")]
        public decimal PriceChange { get; set; }

        /// <summary>
        /// Total number of trades executed during the day.
        /// </summary>
        [JsonPropertyName("zTotTran")]
        [JsonConverter(typeof(DoubleToLongConverter))]
        public long TotalTrades { get; set; }

        /// <summary>
        /// Total trading volume (number of shares traded).
        /// </summary>
        [JsonPropertyName("qTotTran5J")]
        [JsonConverter(typeof(DoubleToLongConverter))]
        public long TotalVolume { get; set; }

        /// <summary>
        /// Total trading value (in Rials).
        /// </summary>
        [JsonPropertyName("qTotCap")]
        public decimal TotalValue { get; set; }

        /// <summary>
        /// Record identifier.
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Indicates whether the closing price has been calculated.
        /// </summary>
        [JsonPropertyName("iClose")]
        public bool IsClosingPriceCalculated { get; set; }

        /// <summary>
        /// Indicates whether yesterday's closing price has been applied.
        /// </summary>
        [JsonPropertyName("yClose")]
        public bool IsYesterdayCloseApplied { get; set; }

        // ============ Computed Properties ============

        /// <summary>
        /// Date represented as a <see cref="DateTime"/> instance.
        /// </summary>
        [JsonIgnore]
        public DateTime Date => DateTime.ParseExact(
            DateGregorian.ToString(),
            "yyyyMMdd",
            System.Globalization.CultureInfo.InvariantCulture);

        /// <summary>
        /// Price change percentage compared to the previous day.
        /// </summary>
        [JsonIgnore]
        public decimal PriceChangePercent =>
            YesterdayPrice == 0 ? 0 : (PriceChange / YesterdayPrice) * 100;

        /// <summary>
        /// Trading volume expressed in millions of shares.
        /// </summary>
        [JsonIgnore]
        public double VolumeInMillions => TotalVolume / 1_000_000.0;

        /// <summary>
        /// Trading value expressed in billions of Rials.
        /// </summary>
        [JsonIgnore]
        public double ValueInBillions => (double)TotalValue / 1_000_000_000.0;

        /// <summary>
        /// Average price per share across all trades.
        /// </summary>
        [JsonIgnore]
        public decimal AveragePrice => TotalVolume == 0 ? 0 : TotalValue / TotalVolume;
    }
}