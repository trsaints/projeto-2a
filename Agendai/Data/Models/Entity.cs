using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Agendai.Data.Models;


public abstract class Entity : INotifyPropertyChanged
{
	public ulong Id { get; set; }

	private string _name;
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	protected Entity(ulong id, string name)
	{
		Id = id;
		_name = name;
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
