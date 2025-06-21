using System;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class HomeWindowViewModel : ViewModelBase
{
	private string[] _selectedListNames = [];

	public string[] SelectedListNames
	{
		get => _selectedListNames;
		set => SetProperty(ref _selectedListNames, value);
	}

	public void AddSelectedListName(string listName)
	{
		if (!_selectedListNames.Contains(listName))
		{
			SelectedListNames = [.. _selectedListNames, listName];
		}
	}

	public void RemoveSelectedListName(string listName)
	{
		if (_selectedListNames.Contains(listName))
		{
			SelectedListNames =
			[
				.. _selectedListNames.Where(name => name != listName)
			];
		}
	}
}
