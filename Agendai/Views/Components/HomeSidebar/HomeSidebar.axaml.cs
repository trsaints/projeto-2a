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

		private void OnCalendarDateChanged(object? sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count > 0 && e.AddedItems[0] is DateTime selectedDate)
			{
				WeakReferenceMessenger.Default.Send(new NavigateToDateMessenger(selectedDate));
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
						if (!_selectedItems.Contains(name)) { _selectedItems.Add(name); }
					}
					else { _selectedItems.Remove(name); }

					WeakReferenceMessenger.Default.Send(
						new GetListsNamesMessenger(_selectedItems.ToArray())
					);
				}
			}
		}

		private void OnChangingListsVisibility(object? sender, RoutedEventArgs e)
		{
			if (sender is Button button
			    && button.DataContext is HomeWindowViewModel homeWindowViewModel)
			{
				if (button.Tag is EventListViewModel eventListViewModel)
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
		}

		private void CheckBoxLoaded(object? sender, RoutedEventArgs e)
		{
			if (sender is CheckBox cb)
			{
				var name = cb.DataContext switch
				{
					TodosByListName todo => todo.ListName,
					EventsByAgenda ev    => ev.AgendaName,
					_                    => null
				};

				if (name != null && cb.IsChecked == true) { _selectedItems.Add(name); }
			}
		}
	}
}
