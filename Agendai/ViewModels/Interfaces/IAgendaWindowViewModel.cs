using System;
using System.Collections.ObjectModel;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.ViewModels.Interfaces;

public interface IAgendaWindowViewModel
{
	public int      SelectedIndex     { get; set; }
	public string[] SelectedListNames { get; }
	public bool     ShowData          { get; }
	public string   SelectedMonth     { get; set; }
	public string   SelectedWeek      { get; set; }
	public string   SelectedDay       { get; set; }
	public DateTime CurrentMonth      { get; set; }
	public DateTime CurrentWeek       { get; set; }
	public DateTime CurrentDay        { get; set; }
	public string   SearchText        { get; set; }

	public ObservableCollection<string>   SearchableItems { get; set; }
	public ObservableCollection<MonthRow> MonthViewRows   { get; }
	public ObservableCollection<WeekRow>  WeekViewRows    { get; }
	public ObservableCollection<DayRow>   DayViewRows     { get; }

	public void GoToPreviousMonth();
	public void GoToNextMonth();
	public void GoToPreviousWeek();
	public void GoToNextWeek();
	public void GoToPreviousDay();
	public void GoToNextDay();
	public void GoToDay(int date);
	public void ToggleShowData();
	public void EditEvent(Event ev);
	public void EditTodo(Todo   todo);
	public void UpdateDataGridItems();
}
