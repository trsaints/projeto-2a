using System.Collections.Generic;
using Agendai.Data.Models;

namespace Agendai.Data.Dtos;

public class TodosByListName
{
	public string ListName { get; set; }

	public IEnumerable<Todo> Items { get; set; }
}
