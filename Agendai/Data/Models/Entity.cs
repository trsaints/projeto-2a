using System.ComponentModel.DataAnnotations;

namespace Agendai.Data.Models;


public abstract class Entity(ulong id, string name)
{
	[Key]
	public ulong  Id   { get; set; } = id;

	[Required]
	[StringLength(128)]
	public string Name { get; set; } = name;
}
