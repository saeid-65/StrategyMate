using StrategyMate.Models;
using System.Text.Json;

namespace StrategyMate.Services
{
    public class TsetmcService : ITsetmcService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TsetmcService> _logger;

        public TsetmcService(HttpClient httpClient, ILogger<TsetmcService> logger)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // ============================================================
        // Retrieve daily closing prices
        // ============================================================
        public async Task<List<DailyClosingPrice>> GetClosingPricesAsync(string instrumentId, int count = 0)
        {
            if (string.IsNullOrWhiteSpace(instrumentId))
            {
                _logger.LogWarning("GetClosingPricesAsync was called with an empty instrumentId.");
                return new List<DailyClosingPrice>();
            }

            var url = $"https://cdn.tsetmc.com/api/ClosingPrice/GetClosingPriceDailyList/{instrumentId}/{count}";

            _logger.LogInformation(
                "Fetching daily closing prices from TSETMC. InstrumentId: {InstrumentId}, Count: {Count}, Url: {Url}",
                instrumentId, count, url);

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var result = JsonSerializer.Deserialize<ClosingPriceDailyResponse>(json, options);
                var prices = result?.ClosingPrices ?? new List<DailyClosingPrice>();

                _logger.LogInformation(
                    "Successfully retrieved {Count} daily closing price records for InstrumentId: {InstrumentId}.",
                    prices.Count, instrumentId);

                return prices;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex,
                    "HTTP error while fetching daily closing prices for InstrumentId: {InstrumentId}.",
                    instrumentId);
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex,
                    "JSON deserialization error while processing daily closing prices for InstrumentId: {InstrumentId}.",
                    instrumentId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unexpected error while fetching daily closing prices for InstrumentId: {InstrumentId}.",
                    instrumentId);
                throw;
            }
        }
    }
}