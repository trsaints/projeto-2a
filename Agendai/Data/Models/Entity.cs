namespace Agendai.Models;


public abstract class Entity(ulong id, string name)
{
	public ulong  Id   { get; set; } = id;
	public string Name { get; set; } = name;
}
