using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrategyMate.Models;

/// <summary>
/// Represents a trading instrument (symbol) on the Tehran Stock Exchange.
/// Maps to the SYMBOLS table in SQLite.
/// </summary>
[Table("SYMBOLS")]
public class Symbol
{
    /// <summary>
    /// Surrogate primary key. Auto-incremented by SQLite
    /// (INTEGER PRIMARY KEY is an alias for ROWID).
    /// </summary>
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    /// <summary>
    /// Unique TSETMC instrument code (insCode). Up to 17 digits,
    /// stored as TEXT because SQLite has no fixed-length numeric type.
    /// </summary>
    [Column("INSTRUMENT_CODE")]
    [MaxLength(20)]
    public string InstrumentCode { get; set; } = string.Empty;

    /// <summary>
    /// Trading symbol as displayed on the exchange (e.g., فولاد, خساپا).
    /// </summary>
    [Column("SYMBOL")]
    [MaxLength(50)]
    public string SymbolName { get; set; } = string.Empty;

    /// <summary>
    /// Full registered company name.
    /// </summary>
    [Column("COMPANY_NAME")]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    /// <summary>
    /// International Securities Identification Number.
    /// Optional; may not be available for all instruments.
    /// </summary>
    [Column("ISIN")]
    [MaxLength(20)]
    public string? Isin { get; set; }

    /// <summary>
    /// FK to INDUSTRY_GROUPS.ID. Nullable because some symbols may not
    /// be classified yet.
    /// </summary>
    [Column("GROUP_ID")]
    public long? GroupId { get; set; }

    /// <summary>
    /// FK to INDUSTRY_SUB_GROUPS.ID.
    /// </summary>
    [Column("SUB_GROUP_ID")]
    public long? SubGroupId { get; set; }

    /// <summary>
    /// Market segment: BOURS | FARA | PAYEH.
    /// Enforced by CHK_SYMBOLS_MARKET check constraint.
    /// </summary>
    [Column("MARKET")]
    [MaxLength(20)]
    public string? Market { get; set; }

    /// <summary>
    /// Board type: MAIN | SUB | BASE.
    /// Enforced by CHK_SYMBOLS_BOARD check constraint.
    /// </summary>
    [Column("BOARD")]
    [MaxLength(20)]
    public string? Board { get; set; }

    /// <summary>
    /// 1 = active/tradable, 0 = inactive.
    /// Stored as INTEGER (SQLite has no native boolean type).
    /// </summary>
    [Column("IS_ACTIVE")]
    public int IsActive { get; set; } = 1;

    /// <summary>
    /// Nominal (par) value of each share, typically 1000 Rials.
    /// </summary>
    [Column("PAR_VALUE")]
    public decimal ParValue { get; set; } = 1000m;

    /// <summary>
    /// Total number of shares issued by the company.
    /// </summary>
    [Column("TOTAL_SHARES")]
    public long? TotalShares { get; set; }

    /// <summary>
    /// Set by SQLite's DEFAULT CURRENT_TIMESTAMP on insert.
    /// </summary>
    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Auto-updated by the TRG_SYMBOLS_UPDATED_AT trigger on every UPDATE.
    /// </summary>
    [Column("UPDATED_AT")]
    public DateTime UpdatedAt { get; set; }

    // ================= Navigation Properties =================

    /// <summary>
    /// Parent industry group (if assigned).
    /// </summary>
    [ForeignKey(nameof(GroupId))]
    public IndustryGroup? Group { get; set; }

    /// <summary>
    /// Parent industry sub-group (if assigned).
    /// </summary>
    [ForeignKey(nameof(SubGroupId))]
    public IndustrySubGroup? SubGroup { get; set; }
}