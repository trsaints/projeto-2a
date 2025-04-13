using System.Windows.Input;
using Agendai.ViewModels;
using Avalonia.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.Input;

namespace Agendai.Views.Components.SideBar;


public partial class SideBar : UserControl, INotifyPropertyChanged
{
	public SideBar()
	{
		InitializeComponent();
		DataContext = new TodoWindowViewModel();
	}
	
}

