using System;
using System.Collections.Generic;
using System.Linq;
using Agendai.Models;
using Agendai.ViewModels;


namespace Agendai.Data;

public class TodosByListName : ViewModelBase
{
	public string ListName { get; set; }

	public IEnumerable<Todo> Items { get; set; }
	
}
