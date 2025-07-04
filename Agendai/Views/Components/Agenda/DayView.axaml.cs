﻿using System.Linq;
using Agendai.ViewModels;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;


namespace Agendai.Views.Components.Agenda;

public partial class DayView : UserControl
{
	public DayView() { InitializeComponent(); }

	private void OnPreviousDayClicked(object? sender, RoutedEventArgs e)
	{
		if (DataContext is AgendaWindowViewModel vm)
			vm.GoToPreviousDay();
	}

	private void OnNextDayClicked(object? sender, RoutedEventArgs e)
	{
		if (DataContext is AgendaWindowViewModel vm)
			vm.GoToNextDay();
	}

	private void ForwardClickToParent(object? sender, RoutedEventArgs e)
	{
		var parentAgenda = this.FindAncestorOfType<Agenda>();
		parentAgenda?.OnEventOrTodoCLicked(sender);
	}

	private void SearchText_OnGotFocus(object? sender, GotFocusEventArgs e)
	{
		if (sender is null) return;

		var autoComplete = (AutoCompleteBox)sender;
		autoComplete.IsDropDownOpen = true;
	}

	private void SearchBox_TextChanged(object? sender, RoutedEventArgs e)
	{
		if (sender is not AutoCompleteBox box) return;

		if (DataContext is AgendaWindowViewModel vm)
		{
			vm.SearchText = box.Text ?? string.Empty;
		}
	}

	private void SearchBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
	{
		if (sender is not AutoCompleteBox { SelectedItem: string selectedName }) return;

		var viewModel = DataContext as AgendaWindowViewModel;

		var ev   = viewModel?.EventList?.Events.FirstOrDefault(x => x.Name == selectedName);
		var todo = viewModel?.TodoList?.Todos.FirstOrDefault(x => x.Name == selectedName);

		if (ev is null && todo is null) return;

		if (todo is null) return;

		Button fakeButton   = new() { Tag = ev ?? (object)todo };
		var    parentAgenda = this.FindAncestorOfType<Agenda>();

		parentAgenda?.OnEventOrTodoCLicked(fakeButton);
	}
}
