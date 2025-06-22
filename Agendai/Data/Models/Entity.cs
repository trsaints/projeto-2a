using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;


namespace Agendai.Data.Models;

public class Entity : INotifyPropertyChanged
{
	#region Entity State

	[NotMapped]
	private string _name;

	#endregion


	[Key]
	public int Id { get; }

	protected Entity() { }

	protected Entity(int id, string name)
	{
		Id    = id;
		_name = name;
	}


	#region State Tracking

	[Required]
	[StringLength(128)]
	public string Name
	{
		get => _name;
		set => SetProperty(ref _name, value);
	}

	public event PropertyChangedEventHandler? PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
		=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

	protected void SetProperty<T>(
		ref T                      storage,
		T                          value,
		[CallerMemberName] string? propertyName = null
	)
	{
		if (EqualityComparer<T>.Default.Equals(storage, value)) return;

		storage = value;
		OnPropertyChanged(propertyName);
	}

	#endregion
}
