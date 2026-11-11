using StrategyMate.Models;

namespace StrategyMate.Services;

/// <summary>
/// Read-only access to the SYMBOLS, INDUSTRY_GROUPS, and INDUSTRY_SUB_GROUPS
/// tables in SQLite. Reference data is maintained manually by the user;
/// this service therefore exposes only query methods, no write/sync logic.
/// </summary>
public interface ISymbolService
{
    /// <summary>
    /// Returns a paged, filtered, and sorted list of symbols.
    /// </summary>
    /// <param name="query">
    /// Contains search term, active-only flag, sort column, sort direction,
    /// and paging parameters.
    /// </param>
    Task<PagedResult<Symbol>> GetSymbolsAsync(SymbolQuery query);

    /// <summary>
    /// Returns a single symbol by its TSETMC instrument code (insCode),
    /// or <c>null</c> if no match exists. Navigation properties
    /// (Group, SubGroup) are eagerly loaded.
    /// </summary>
    Task<Symbol?> GetSymbolAsync(string instrumentCode);

    /// <summary>
    /// Returns aggregate counts for the symbols table
    /// (total, active, inactive). Used to render the stat cards
    /// at the top of the Symbols page.
    /// </summary>
    Task<SymbolStats> GetStatsAsync();
}

/// <summary>
/// Aggregate statistics about the SYMBOLS table.
/// </summary>
/// <param name="Total">Total number of rows.</param>
/// <param name="Active">Rows where IS_ACTIVE = 1.</param>
/// <param name="Inactive">Rows where IS_ACTIVE = 0.</param>
public record SymbolStats(int Total, int Active, int Inactive);