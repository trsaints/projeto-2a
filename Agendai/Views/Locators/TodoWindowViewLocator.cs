using Agendai.Data.Repositories.DesignTime;
using Agendai.Data.Repositories.Interfaces;
using Agendai.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Agendai.Views.Locators;

public class TodoWindowViewLocator
{
    public static TodoWindowViewModel ViewModel =>
        App.ServiceProvider?.GetService<ITodoRepository>() 
        is ITodoRepository repo
            ? new TodoWindowViewModel(repo)
            : new TodoWindowViewModel(new TodoDesignTimeRepository());
}
