using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrategyMate.Models;

[Table("INDUSTRY_GROUPS")]
public class IndustryGroup
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("GROUP_NAME")]
    [MaxLength(100)]
    public string GroupName { get; set; } = string.Empty;

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    public ICollection<IndustrySubGroup> SubGroups { get; set; } = new List<IndustrySubGroup>();
    public ICollection<Symbol> Symbols { get; set; } = new List<Symbol>();
}