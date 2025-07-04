﻿using System.Windows.Input;
using Agendai.ViewModels.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;


namespace Agendai.ViewModels;

public class ViewModelBase : ObservableObject, IViewModelBase
{
	public MainWindowViewModel? MainViewModel { get; set; }

	private ICommand? _returnHomeCommand;

	public ICommand ReturnHomeCommand => _returnHomeCommand ??= new RelayCommand(ReturnHome);

	private void ReturnHome() { MainViewModel?.NavigateToHome(); }
}
