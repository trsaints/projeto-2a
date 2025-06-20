using System;
using System.Linq;

namespace Agendai.Messages;

public class GetListsNamesMessenger
{
    public string?[] SelectedItemsName { get; }

    public GetListsNamesMessenger(string?[] listNames)
    {
        SelectedItemsName = listNames;
    }
}
