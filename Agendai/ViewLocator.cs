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

        Control result = param switch
        {
            HomeWindowViewModel => new HomeWindow(),
            AgendaWindowViewModel => new AgendaWindow(),
            TodoWindowViewModel => new TodoWindow(),
            _ => throw new ArgumentException($"No view found for {param.GetType().Name}"),
        };

        return result;
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}
