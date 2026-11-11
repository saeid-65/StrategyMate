using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StrategyMate.Models;

[Table("INDUSTRY_SUB_GROUPS")]
public class IndustrySubGroup
{
    [Key]
    [Column("ID")]
    public long Id { get; set; }

    [Column("SUB_GROUP_NAME")]
    [MaxLength(100)]
    public string SubGroupName { get; set; } = string.Empty;

    [Column("GROUP_ID")]
    public long GroupId { get; set; }

    [Column("CREATED_AT")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey(nameof(GroupId))]
    public IndustryGroup? Group { get; set; }

    public ICollection<Symbol> Symbols { get; set; } = new List<Symbol>();
}