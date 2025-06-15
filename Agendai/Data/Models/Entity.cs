using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;

namespace Agendai.Data.Models;


public class Entity : INotifyPropertyChanged
{
    [NotMapped]
    private string? _name;

    public Entity(ulong id, string name)
    {
        Id = id;
        _name = name;
    }

    public Entity() { }

    [Key]
    public ulong Id { get; set; }


    [Required]
    [StringLength(128)]
    public string Name
    {
        get => _name ?? string.Empty;
        set => SetProperty(ref _name, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value))
            return false;

        storage = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
