using System.ComponentModel.DataAnnotations;

namespace Agendai.Data.Models;


public abstract class Entity()
{
    [Key]
    public ulong Id { get; set; }

    [Required]
    [StringLength(128)]
    public string? Name { get; set; }
}
