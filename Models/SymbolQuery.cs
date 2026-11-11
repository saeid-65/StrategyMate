namespace StrategyMate.Models;

/// <summary>
/// Encapsulates all filtering, sorting, and paging parameters used when
/// querying the symbols list. Passed from the UI to <c>ISymbolService</c>
/// so that the service doesn't need to know about component state.
/// </summary>
public class SymbolQuery
{
    /// <summary>
    /// Free-text search term. Matched against symbol name, company name,
    /// and instrument code.
    /// </summary>
    public string? Search { get; set; }

    /// <summary>
    /// When true, only symbols with IS_ACTIVE = 1 are returned.
    /// </summary>
    public bool OnlyActive { get; set; } = true;

    /// <summary>
    /// Column to sort by. Supported: Symbol, Company, Code.
    /// Unknown values fall back to Symbol.
    /// </summary>
    public string SortBy { get; set; } = "Symbol";

    /// <summary>
    /// Sort direction. True = ascending, false = descending.
    /// </summary>
    public bool SortAscending { get; set; } = true;

    /// <summary>
    /// 1-based page number.
    /// </summary>
    public int Page { get; set; } = 1;

    /// <summary>
    /// Number of items per page. Clamped in the service (5–200).
    /// </summary>
    public int PageSize { get; set; } = 25;
}

/// <summary>
/// A paged result set: the current page's items plus the total count so
/// the caller can compute page numbers and disable navigation buttons.
/// </summary>
public class PagedResult<T>
{
    /// <summary>Items on the current page.</summary>
    public List<T> Items { get; set; } = new();

    /// <summary>Total number of matching rows across all pages.</summary>
    public int TotalCount { get; set; }

    /// <summary>Current 1-based page number.</summary>
    public int Page { get; set; }

    /// <summary>Page size used for this result.</summary>
    public int PageSize { get; set; }

    /// <summary>Total number of pages (ceil(TotalCount / PageSize)).</summary>
    public int TotalPages =>
        PageSize <= 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>True if a previous page exists.</summary>
    public bool HasPrevious => Page > 1;

    /// <summary>True if a next page exists.</summary>
    public bool HasNext => Page < TotalPages;
}