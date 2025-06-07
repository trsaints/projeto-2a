using Agendai.Data.Models;
using Agendai.Data.Repositories.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Agendai.Utils;

public static class DatabaseUtils
{
    public static async Task MapToObservable<T>(
        IRepository<T> repository,
        ObservableCollection<T> targetCollection) where T : Entity
    {
        var items = await repository.ReadAll();

        targetCollection.Clear();

        foreach (var item in items ?? []) targetCollection.Add(item);
    }
}
