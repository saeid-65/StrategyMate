using StrategyMate.Models;

namespace StrategyMate.Services
{
    public interface ITsetmcService
    {
        /// <summary>
        /// Retrieves the list of daily closing prices for a given instrument.
        /// </summary>
        Task<List<DailyClosingPrice>> GetClosingPricesAsync(string instrumentId, int count = 0);
    }
}
