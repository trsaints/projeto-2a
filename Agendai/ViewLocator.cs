using System;
using Agendai.ViewModels;
using Agendai.Views.Windows.AgendaWindow;
using Agendai.Views.Windows.HomeWindow;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Threading;

namespace Agendai;

public class ViewLocator : IDataTemplate
{
    public Control Build(object param)
    {
        if (param is null)
            return new TextBlock { Text = "No ViewModel provided" };
            
        Control result;
        
        if (param is HomeWindowViewModel)
            result = new HomeWindow();
        else if (param is AgendaWindowViewModel)
            result = new AgendaWindow();
        else
        {
            var name = param.GetType().Name.Replace("ViewModel", "");
            var type = Type.GetType($"Agendai.Views.{name}");
            
            if (type != null)
                result = (Control)Activator.CreateInstance(type)!;
            else
                result = new TextBlock { Text = $"View not found for {param.GetType().FullName}" };
        }
        
        return result;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
