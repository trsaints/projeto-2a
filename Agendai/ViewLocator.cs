using System;
using Agendai.ViewModels;
using Avalonia.Controls;
using Avalonia.Controls.Templates;


namespace Agendai;


public class ViewLocator : IDataTemplate
{
	public Control Build(object param)
	{
		var viewModelType = param.GetType();
		
		// assure the string matches the naming convention, for avoiding
		// inconsistencies such as "HomeWindowWindow" in the context search
		// after replacing "ViewModel" with "Window"
		
		var windowName = viewModelType.Name.Replace("ViewModel", "Window")
		                              .Replace("WindowWindow", "Window"); 
		
		// windowName is doubled because each window is within its own namespace
		// example: Agendai.Views.Windows.HomeWindow.HomeWindow
		var fullNamespace = $"Agendai.Views.Windows.{windowName}.{windowName}";
		var viewType      = Type.GetType(fullNamespace);

		if (viewType is not null)
		{
			return (Control)Activator.CreateInstance(viewType)!;
		}

		return new TextBlock { Text = $"View not found for {fullNamespace}" };
	}

	public bool Match(object param) { return param is ViewModelBase; }
}
