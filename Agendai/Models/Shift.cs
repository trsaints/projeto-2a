using System;
using System.Collections;
using System.Collections.Generic;


namespace Agendai.Models;


public class Shift(ulong id, string name) : Entity(id, name)
{
	public TimeOnly Duration { get; set; }
	public ShiftStatus Status { get; set; }
	
	//Relacao como o todo
	public ulong TodoId { get; set; }

	public ICollection<Todo> Todos{get; set;} = new List<Todo>();
}
