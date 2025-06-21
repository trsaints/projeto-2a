using System.ComponentModel;
using System;
using System.Collections.Generic;


namespace Agendai.Data.Models;

public class Todo(ulong id, string name)
		: Recurrence(id, name), INotifyPropertyChanged
{
	public         string? ListName       { get; set; }
	public         uint    FinishedShifts { get; set; }
	public         uint    TotalShifts    { get; set; }
	public virtual Event?  RelatedEvent   { get; set; }

	private TodoStatus _status;
	public TodoStatus Status
	{
		get => _status;

		set
		{
			if (_status != value)
			{
				_status = value;
				OnPropertyChanged(nameof(Status));
				OnStatusChanged?.Invoke(this, _status);
			}
		}
	}

	public string? SkippedDisplay =>
			Status == TodoStatus.Skipped ? " (pulado)" : string.Empty;

	public new event PropertyChangedEventHandler? PropertyChanged;
	public event     Action<Todo, TodoStatus>?    OnStatusChanged;

	protected new virtual void OnPropertyChanged(string propertyName)
	{
		PropertyChanged?.Invoke(
			this,
			new PropertyChangedEventArgs(propertyName)
		);
	}

	public static List<Todo> Samples()
	{
		return
		[
			new Todo(1, "Comprar p�o")
			{
				ListName    = "Casa", Status    = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			},
			new Todo(2, "Lavar a lou�a")
			{
				ListName    = "Casa", Status    = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			},
			new Todo(3, "Estudar matem�tica")
			{
				ListName    = "Estudos", Status = TodoStatus.Incomplete,
				TotalShifts = 2, FinishedShifts = 0
			},
			new Todo(4, "Ler cap�tulo de livro")
			{
				ListName    = "Estudos", Status = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			},
			new Todo(5, "Enviar relat�rio")
			{
				ListName    = "Trabalho", Status = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts  = 0
			},
			new Todo(6, "Reuni�o com equipe")
			{
				ListName    = "Trabalho", Status = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts  = 0
			},
			new Todo(7, "Passear com o cachorro")
			{
				ListName    = "Casa", Status    = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			},
			new Todo(8, "Revisar anota��es")
			{
				ListName    = "Estudos", Status = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			},
			new Todo(9, "Organizar documentos")
			{
				ListName    = "Trabalho", Status = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts  = 0
			},
			new Todo(10, "Limpar o quarto")
			{
				ListName    = "Casa", Status    = TodoStatus.Incomplete,
				TotalShifts = 1, FinishedShifts = 0
			}
		];
	}
}
