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
	
	private string[] _selectedListNames = Array.Empty<string>();


	public string[] SelectedListNames
	{
		get => _selectedListNames;
		set => SetProperty(ref _selectedListNames, value);
	}
	
	public void AddSelectedListName(string listName)
	{
		if (!_selectedListNames.Contains(listName))
		{
			SelectedListNames = _selectedListNames.Concat(new[] { listName }).ToArray();
		}
	}
	public void RemoveSelectedListName(string listName)
	{
		if (_selectedListNames.Contains(listName))
		{
			SelectedListNames = _selectedListNames.Where(name => name != listName).ToArray();
		}
	}
}
