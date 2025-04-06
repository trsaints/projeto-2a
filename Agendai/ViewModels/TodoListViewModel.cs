using System;
using System.Collections.ObjectModel;
using System.Linq;
using Agendai.Models;
using System.Collections.Generic;
using DynamicData;


namespace Agendai.ViewModels;


public class TodoListViewModel : ViewModelBase
{
	public TodoListViewModel()
	{
		CompletedTasks = new ObservableCollection<Todo>();
		
		Todos = [
            new Todo(1, "Comprar Café")
            {
                Description = "Preciso comprar café",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Monthly,
                Status = TodoStatus.Complete
            },
            new Todo(2, "Comprar Pão")
            {
                Description = "Preciso comprar pão",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Weekly
            },
            new Todo(3, "Comprar Leite")
            {
                Description = "Preciso comprar leite",
                Due = new DateTime(2025, 03, 31),
                Repeats = Repeats.Daily
            },
            new Todo(4, "Pagar Conta de Luz")
            {
                Description = "Vencimento da conta de energia",
                Due = new DateTime(2025, 04, 10),
                Repeats = Repeats.Monthly
            },
            new Todo(5, "Levar cachorro ao veterinário")
            {
                Description = "Consulta de rotina para o Rex",
                Due = new DateTime(2025, 04, 15),
                Repeats = Repeats.Weekly
            },
            new Todo(6, "Estudar para prova de Estrutura de Dados")
            {
                Description = "Revisar listas encadeadas e pilhas",
                Due = new DateTime(2025, 04, 07),
                Repeats = Repeats.None,
                Status = TodoStatus.Complete
            },
            new Todo(7, "Fazer trabalho de Arquitetura de Computadores")
            {
                Description = "Pesquisar sobre pipeline de instruções",
                Due = new DateTime(2025, 04, 10),
                Repeats = Repeats.None
            },
            new Todo(8, "Terminar projeto de Programação")
            {
                Description = "Finalizar implementação e revisar código",
                Due = new DateTime(2025, 04, 14),
                Repeats = Repeats.None
            },
            new Todo(9, "Revisar Teoria da Computação")
            {
                Description = "Estudar autômatos finitos e gramáticas formais",
                Due = new DateTime(2025, 04, 15),
                Repeats = Repeats.None,
                Status = TodoStatus.Complete
            },
            new Todo(10, "Resolver exercícios de Cálculo I")
            {
                Description = "Praticar derivadas e limites",
                Due = new DateTime(2025, 04, 17),
                Repeats = Repeats.None
            },
            new Todo(11, "Ir ao supermercado")
            {
                Description = "Comprar arroz, feijão e carne",
                Due = new DateTime(2025, 04, 05),
                Repeats = Repeats.Weekly
            },
            new Todo(12, "Lavar o carro")
            {
                Description = "Dar uma geral no carro no final de semana",
                Due = new DateTime(2025, 04, 06),
                Repeats = Repeats.Weekly
            },
            new Todo(13, "Ler um capítulo do livro de Machine Learning")
            {
                Description = "Entender redes neurais profundas",
                Due = new DateTime(2025, 04, 08),
                Repeats = Repeats.None
            },
            new Todo(14, "Fazer exercícios de lógica de programação")
            {
                Description = "Resolver problemas em C#",
                Due = new DateTime(2025, 04, 09),
                Repeats = Repeats.None
            },
            new Todo(15, "Renovar identidade")
            {
                Description = "Agendar atendimento no Poupatempo",
                Due = new DateTime(2025, 04, 20),
                Repeats = Repeats.None
            }
        ];

		
		CompletedTasksTodo();
	}
	
	public ObservableCollection<Todo> Todos { get; set; }
	public ObservableCollection<Todo> CompletedTasks { get; set; }
	
	public bool IsComplete(Todo todo)
	{
		return todo.Status is TodoStatus.Complete;
	}
	private void CompletedTasksTodo()
	{
		foreach (var todo in Todos.ToList())
		{
			if (IsComplete(todo))
			{
				CompletedTasks.Add(todo); 
				Todos.Remove(todo);
			}
		}
	}
}
