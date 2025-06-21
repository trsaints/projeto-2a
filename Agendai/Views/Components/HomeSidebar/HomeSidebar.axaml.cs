using System;
using System.Collections.Generic;
using System.Linq;
using Agendai.Messages;
using Avalonia;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.Messaging;
using Agendai.ViewModels;
using Avalonia.Interactivity;
using Agendai.Data;


namespace Agendai.Views.Components.HomeSidebar
{
	public partial class HomeSidebar : UserControl
	{
		private readonly List<string> _selectedItems = new();

		public HomeSidebar() { InitializeComponent(); }

		private void OnCalendarDateChanged(
			object?                   sender,
			SelectionChangedEventArgs e
		)
		{
			if (e.AddedItems.Count > 0
			    && e.AddedItems[0] is DateTime selectedDate)
			{
				WeakReferenceMessenger.Default.Send(
					new NavigateToDateMessenger(selectedDate)
				);
			}
		}

		private void OnCheckBoxChecked(object? sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox)
			{
				string? name = null;

				if (checkBox.DataContext is TodosByListName todoList)
				{
					name = todoList.ListName;
				}
				else if (checkBox.DataContext is EventsByAgenda eventAgenda)
				{
					name = eventAgenda.AgendaName;
				}

				if (name != null)
				{
					if (checkBox.IsChecked == true)
					{
						if (!_selectedItems.Contains(name))
						{
							_selectedItems.Add(name);
						}
					}
					else { _selectedItems.Remove(name); }

					WeakReferenceMessenger.Default.Send(
						new GetListsNamesMessenger(_selectedItems.ToArray())
					);
				}
			}
		}

		private void CheckBoxChecked(object? sender, RoutedEventArgs e)
		{
			if (sender is CheckBox checkBox
			    && checkBox.DataContext is string listName)
			{
				if (checkBox.IsChecked == true)
				{
					if (!_selectedItems.Contains(listName))
						_selectedItems.Add(listName);
				}
				else { _selectedItems.Remove(listName); }

				if (_selectedItems.Count == 0
				    && DataContext is HomeWindowViewModel vm)
				{
					var allNames = vm.TodoWindowVm.ListNames.ToArray();
					WeakReferenceMessenger.Default.Send(
						new GetListsNamesMessenger(allNames)
					);
				}
				else
				{
					WeakReferenceMessenger.Default.Send(
						new GetListsNamesMessenger(_selectedItems.ToArray())
					);
				}
			}
		}
	}
}
