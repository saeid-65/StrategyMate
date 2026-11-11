using Microsoft.EntityFrameworkCore;
using StrategyMate.Data;
using StrategyMate.Models;

namespace StrategyMate.Services;

/// <summary>
/// EF Core-backed implementation of <see cref="ISymbolService"/>.
/// Uses <see cref="IDbContextFactory{TContext}"/> rather than a scoped
/// DbContext so that concurrent read operations (e.g., from multiple
/// Blazor circuits) don't step on each other.
/// </summary>
public class SymbolService : ISymbolService
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

    public SymbolService(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    // ========================================================================
    // GetSymbolsAsync
    // ========================================================================
    /// <summary>
    /// Builds a paged and filtered list of symbols. Filtering, sorting, and
    /// paging all happen in the database so that only the current page's
    /// rows are materialized.
    /// </summary>
    public async Task<PagedResult<Symbol>> GetSymbolsAsync(SymbolQuery query)
    {
        // A fresh context per call keeps concurrent requests isolated.
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        // Start with a no-tracking query (read-only) and include navigation
        // properties that the UI displays (group and sub-group names).
        var q = db.Symbols
            .AsNoTracking()
            .Include(s => s.Group)
            .Include(s => s.SubGroup)
            .AsQueryable();

        // ---- Filter: active only ----
        // IS_ACTIVE is stored as INTEGER (1/0) because SQLite has no bool type.
        if (query.OnlyActive)
        {
            q = q.Where(s => s.IsActive == 1);
        }

        // ---- Filter: free-text search ----
        // Matches symbol name, company name, or instrument code.
        // SQLite's LIKE is case-insensitive for ASCII by default, which is
        // sufficient here since Persian has no case distinction.
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var term = query.Search.Trim();
            q = q.Where(s =>
                EF.Functions.Like(s.SymbolName, $"%{term}%") ||
                EF.Functions.Like(s.CompanyName, $"%{term}%") ||
                EF.Functions.Like(s.InstrumentCode, $"%{term}%"));
        }

        // Count BEFORE paging so the UI can render "X of Y".
        var total = await q.CountAsync();

        // ---- Sorting ----
        // Whitelist approach: only known column names are honored.
        q = (query.SortBy, query.SortAscending) switch
        {
            ("Company", true)  => q.OrderBy(s => s.CompanyName),
            ("Company", false) => q.OrderByDescending(s => s.CompanyName),
            ("Code", true)     => q.OrderBy(s => s.InstrumentCode),
            ("Code", false)    => q.OrderByDescending(s => s.InstrumentCode),
            (_, true)          => q.OrderBy(s => s.SymbolName),
            (_, false)         => q.OrderByDescending(s => s.SymbolName),
        };

        // ---- Paging ----
        // Clamp values so a malformed query can't request 1M rows.
        var page = Math.Max(1, query.Page);
        var size = Math.Clamp(query.PageSize, 5, 200);

        var items = await q
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PagedResult<Symbol>
        {
            Items = items,
            TotalCount = total,
            Page = page,
            PageSize = size,
        };
    }

    // ========================================================================
    // GetSymbolAsync
    // ========================================================================
    /// <summary>
    /// Loads a single symbol by instrument code. Returns <c>null</c> when
    /// the code is empty or not found. Navigation properties (Group,
    /// SubGroup) are included so the caller can display names without an
    /// extra round-trip.
    /// </summary>
    public async Task<Symbol?> GetSymbolAsync(string instrumentCode)
    {
        if (string.IsNullOrWhiteSpace(instrumentCode))
        {
            return null;
        }

        await using var db = await _dbContextFactory.CreateDbContextAsync();

        return await db.Symbols
            .AsNoTracking()
            .Include(s => s.Group)
            .Include(s => s.SubGroup)
            .FirstOrDefaultAsync(s => s.InstrumentCode == instrumentCode);
    }

    // ========================================================================
    // GetStatsAsync
    // ========================================================================
    /// <summary>
    /// Returns total, active, and inactive counts in a single round-trip
    /// by issuing three lightweight COUNT queries. On small tables this is
    /// negligible; if it ever becomes a hot path, these can be cached.
    /// </summary>
    public async Task<SymbolStats> GetStatsAsync()
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var total = await db.Symbols.CountAsync();
        var active = await db.Symbols.CountAsync(s => s.IsActive == 1);

        return new SymbolStats(total, active, total - active);
    }
}