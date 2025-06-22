using System.Collections.Generic;
using Agendai.Data.Models;
using Agendai.ViewModels;


namespace Agendai.Data.Abstractions;

public class TodosByListName : ViewModelBase
{
	public string ListName { get; set; }

	public IEnumerable<Todo> Items { get; set; }
}
