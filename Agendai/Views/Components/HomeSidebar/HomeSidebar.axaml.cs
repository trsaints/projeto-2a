using System;
using System.Collections.Generic;
using Agendai.Messages;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.ViewModels;
using Avalonia.Interactivity;
using Agendai.Data;


namespace Agendai.Views.Components.HomeSidebar;

public partial class HomeSidebar : UserControl
{
	private readonly List<string> _selectedItems = [];

	public HomeSidebar() { InitializeComponent(); }

	private void OnCalendarDateChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (e.AddedItems.Count > 0 && e.AddedItems[0] is DateTime selectedDate)
		{
			WeakReferenceMessenger.Default.Send(new NavigateToDateMessenger(selectedDate));
		}
	}

	private void OnCheckBoxChecked(object? sender, RoutedEventArgs e)
	{
		if (sender is not CheckBox checkBox) return;

		var name = checkBox.DataContext switch
		{
			TodosByListName todoList   => todoList.ListName,
			EventsByAgenda eventAgenda => eventAgenda.AgendaName,
			_                          => null
		};

		if (name is null) return;

		if (checkBox.IsChecked is true)
		{
			if (!_selectedItems.Contains(name)) { _selectedItems.Add(name); }
		}
		else { _selectedItems.Remove(name); }

		WeakReferenceMessenger.Default.Send(
			new GetListsNamesMessenger(_selectedItems.ToArray())
		);
	}

	private void OnChangingListsVisibility(object? sender, RoutedEventArgs e)
	{
		if (sender is not Button
		    {
			    DataContext: HomeWindowViewModel homeWindowViewModel
		    } button) return;

		if (button.Tag is EventListViewModel)
		{
			homeWindowViewModel.IsEventListsAbleToView =
					!homeWindowViewModel.IsEventListsAbleToView;
		}
		else
		{
			homeWindowViewModel.IsTodoListsAbleToView =
					!homeWindowViewModel.IsTodoListsAbleToView;
		}
	}

	private void CheckBoxLoaded(object? sender, RoutedEventArgs e)
	{
		if (sender is not CheckBox cb) return;

		var name = cb.DataContext switch
		{
			TodosByListName todo => todo.ListName,
			EventsByAgenda ev    => ev.AgendaName,
			_                    => null
		};

		if (name is not null && cb.IsChecked is true) { _selectedItems.Add(name); }
	}
}
