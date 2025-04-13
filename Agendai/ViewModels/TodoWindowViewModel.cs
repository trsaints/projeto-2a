using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System;
using System.ComponentModel;

namespace Agendai.ViewModels;

public class TodoWindowViewModel : ViewModelBase
{
    public string Title { get; set; } = "Tarefas";

    private bool _isPopupOpen;
    public bool IsPopupOpen
    {
        get => _isPopupOpen;
        set
        {
            _isPopupOpen = value;
            OnPropertyChanged();
        }
    }

    private bool _openAddTask;

    public bool OpenAddTask
    {
        get => _openAddTask;
        set
        {
            _openAddTask = value;
            OnPropertyChanged();
        }
    }

    private string _selectedList;
    public string SelectedList
    {
        get => _selectedList;
        set
        {
            if (_selectedList == value)
                return;

            _selectedList = value;
            OnPropertyChanged();

            OpenAddTask = (value == "Tarefa");
        }
    }
    public ICommand OpenPopupCommand { get; }
    public ICommand SelectTarefaCommand { get; }
    
    public TodoListViewModel TodoListVm { get; }

    public TodoWindowViewModel()
    {
        TodoListVm = new TodoListViewModel();
        OpenPopupCommand = new RelayCommand(() => IsPopupOpen = true);
        TodoListVm.OnTaskAdded = () =>
        {
            OpenAddTask = false;
        };
        SelectTarefaCommand = new RelayCommand(() => 
        {
            OpenAddTask = true; 
            IsPopupOpen = false;
        });
    }
}