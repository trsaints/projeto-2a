using System.Collections.Generic;
using Agendai.Models;


namespace Agendai.Data;

public class TodosByListName
{
	public string ListName { get; set; }

	public IEnumerable<Todo> Items { get; set; }
}
