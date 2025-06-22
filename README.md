# 📅 Agendai
Agendai é uma aplicação gráfica desenvolvida com o objetivo de gerenciar compromissos pessoais e profissionais de forma prática e organizada. Voltada para uso individual, permite o registro, edição e consulta de eventos, funcionando como um sistema pessoal de organização de tarefas e agendamentos.

## Sumário

* [Estruturas de Dados](#estruturas-de-dados)
* [Recorrência de Eventos e Tarefas](#recorrência-de-eventos-e-tarefas)
* [Inicializando o Sistema](#inicializando-o-sistema)
* [Requisitos](#requisitos)
* [Estrutura do Sistema](#estrutura-do-sistema)

---

## Estruturas de Dados
Nesse projeto foram utilizadas diversas estruturas de dados, cada uma com uma função específica:

### Vetores
São utilizados para armazenar dados sequenciais que precisam ser mapeados diretamente para a interface, como a lista de horários que preenche as visualizações diárias e semanais.

### Dicionários
Usados para armazenar dados que têm uma chave única, como o número de dias em cada mês. Dessa forma, a transição entre visualizações utiliza o dicionário para saber o limite de dias que cada mês pode ter.

### Listas
Armazenam coleções ordenadas e dinâmicas, como as próximas ocorrências de eventos ou tarefas. Essa estrutura facilita iterar sobre o que precisa acontecer futuramente e preencher o calendário automaticamente.

### Pilhas
Usadas para armazenar ocorrências que já passaram, funcionando como um histórico. Como a pilha segue o princípio LIFO (Last In, First Out), conseguimos acessar facilmente a última ocorrência concluída e gerenciar o que já foi feito.

---

## Recorrência de Eventos e Tarefas
Aqui há um destaque para a utilização das listas e pilhas, pois elas são fundamentais para gerenciar a parte mais complexa do projeto — a recorrência.

- As listas mantêm as próximas ocorrências programadas para cada evento ou tarefa. À medida que o tempo passa e o usuário navega no calendário, essas ocorrências são iteradas e exibidas automaticamente, garantindo que eventos futuros sempre estejam disponíveis.
- Já as pilhas armazenam o histórico de ocorrências que já passaram. Essa estrutura funciona perfeitamente aqui porque segue o princípio LIFO (Last In, First Out), permitindo que a ocorrência mais recente concluída esteja sempre no topo para consulta ou reversão, facilitando a manutenção e auditoria dessas instâncias.

Essa combinação dá flexibilidade para o sistema “enxergar o futuro” (com as listas) e “relembrar o passado” (com as pilhas), tornando o gerenciamento da recorrência eficiente e organizado.

![recurrence-example.png](./Agendai/Assets/recurrence-exampl.png)

A imagem acima traz um exemplo direto da recorrência da tarefa "Treino Fullbody", que é diária e só deixa de aparecer caso esteja "Completa" 

---

## Inicializando o Sistema
O Agendai não requer variáveis de ambiente nem configuração prévia. Todo o gerenciamento de dados é realizado localmente por meio de um banco de dados SQLite, que será criado automaticamente na primeira execução do sistema.

#### Pré-requisitos
* .NET SDK 8.0 ou superior instalado na máquina.
* Avalonia UI 11.2.1
* Sistema operacional Windows, Linux ou macOS (multiplataforma via Avalonia UI).

### Clonando o repositório

Para obter o código-fonte e preparar o ambiente de execução:

```
git clone https://github.com/trsaints/projeto-2a.git
cd agendai
```

### Clonando o repositório

Com o SDK instalado e o repositório clonado, basta executar:
```
dotnet run
```

## Requisitos
O Agendai atende aos seguintes requisitos funcionais:
* Gerenciar compromissos (eventos e tarefas):
  > Cadastro de compromissos com título, descrição, data e hora;

  > Edição de compromissos existentes;

  > Exclusão de compromissos;

  > Listagem de compromissos ordenados por data/hora.

* Classificação por categorias:

  > Permite criação de categorias personalizadas para organização dos compromissos;

  > Possibilidade de edição e exclusão de categorias.

* Persistência de dados:

  > Todas as alterações são persistidas localmente no banco de dados SQLite;

  > Os dados são armazenados de forma segura, sem risco de perda em desligamentos inesperados.

* Interface gráfica:

  > Interface intuitiva e responsiva desenvolvida em Avalonia UI;

  > Suporte a temas e fontes customizadas.

* Operação offline:

  > Não depende de conexão com internet para funcionamento.

## Tecnologias Utilizadas

O desenvolvimento do Agendai utilizou as seguintes tecnologias:

- **.NET 8.0** — Plataforma base de desenvolvimento.
- **C#** — Linguagem de programação principal.
- **Avalonia UI** — Framework multiplataforma para a interface gráfica desktop.
- **SQLite** — Banco de dados leve e local para persistência das informações.
- **MVVM** — Padrão arquitetural utilizado na estruturação do sistema.


## 🗄️ Modelagem

As entidades e relacionamentos do sistema **Agendai** foram definidas para permitir o gerenciamento de compromissos, tarefas e agendas com suporte a recorrência, lembretes e exibição visual em calendário.

Toda a modelagem de dados se encontra em `Agendai.Data.Models`.

---

### Visão Geral das Entidades

As principais entidades do sistema são:

- Entity (Base Abstrata)
- Recurrence (Base para Eventos Recorrentes)
- Event (Evento de Agenda)
- Todo (Tarefa Associada)
- Shift (Turno de Execução de Tarefas)
- DayCell, DayRow, WeekRow, MonthRow (estrutura de visualização de calendário)
- Repeats (Enumeração de Recorrência)
- ShiftStatus (Enumeração de Status de Turno)
- TodoStatus (Enumeração de Status de Tarefa)

![data-model.png](./Agendai/Assets/data-model.png)

---

### Entity

Classe abstrata base para todas as entidades.

```csharp
public abstract class Entity
{
    public ulong Id { get; set; }
    public string Name { get; set; }
}
```

### Recurrence
Entidade abstrata que adiciona propriedades de recorrência.

```csharp
public abstract class Recurrence : Entity
{
    public Repeats Repeats { get; set; }
    public IEnumerable<DateTime>? Reminders { get; set; }
    public DateTime InitialDue { get; set; }
    public DateTime Due { get; set; }
    public string? Description { get; set; }
}
```
### Event

Representa um evento de agenda, com possibilidade de cor e tarefas relacionadas.

```csharp
public class Event : Recurrence
{
    public string? AgendaName { get; set; }
    public virtual ICollection<Todo>? Todos { get; set; }
    public string? Color { get; set; }
}
```

### Todo

Representa tarefas vinculadas a eventos, com controle de status e turnos.

```csharp
public class Todo : Recurrence
{
    public string? ListName { get; set; }
    public uint FinishedShifts { get; set; }
    public uint TotalShifts { get; set; }
    public virtual Event? RelatedEvent { get; set; }
    public TodoStatus Status { get; set; }
}
```

### Shift
Representa um turno de execução dentro de um Todo.

```csharp
public class Shift : Entity
{
    public TimeOnly Duration { get; set; }
    public ShiftStatus Status { get; set; }
}
```
### DayCell
Representa uma célula do calendário, contendo múltiplos itens (eventos ou tarefas).

```csharp
public class DayCell
{
    public int? DayNumber { get; set; }
    public ObservableCollection<object> Items { get; set; } = new();
}

```
### DayRow
Representa uma linha de dias em uma visualização diária.

```csharp
public class DayRow
{
    public string Hour { get; set; }
    public ObservableCollection<object> Items { get; set; } = new();
}
```
### WeekRow
Representa uma linha semanal, com um DayCell para cada dia da semana.

```csharp
public class WeekRow
{
    public string Hour { get; set; }
    public DayCell Sunday { get; set; } = new();
    public DayCell Monday { get; set; } = new();
    public DayCell Tuesday { get; set; } = new();
    public DayCell Wednesday { get; set; } = new();
    public DayCell Thursday { get; set; } = new();
    public DayCell Friday { get; set; } = new();
    public DayCell Saturday { get; set; } = new();
}
```
### MonthRow
Representa uma linha mensal, com um DayCell para cada dia da semana.

```csharp
public class MonthRow
{
    public DayCell Sunday { get; set; } = new();
    public DayCell Monday { get; set; } = new();
    public DayCell Tuesday { get; set; } = new();
    public DayCell Wednesday { get; set; } = new();
    public DayCell Thursday { get; set; } = new();
    public DayCell Friday { get; set; } = new();
    public DayCell Saturday { get; set; } = new();
}
```
### Repeats
Enumeração que define o padrão de repetição de uma recorrência.

```csharp
public enum Repeats
{
    None,
    Daily,
    Weekly,
    Monthly,
    Anually
}
```
### ShiftStatus
Define o estado de um turno.

```csharp
public enum ShiftStatus
{
    Incomplete,
    Complete,
    Skipped
}
```
### TodoStatus
Define o estado de uma tarefa (Todo).

```csharp
public enum TodoStatus
{
    Incomplete,
    Complete,
    Skipped
}
```

---

## Estrutura do Sistema

Esta seção descreve a estrutura geral do projeto **Agendai**, com a divisão dos principais diretórios, arquivos e responsabilidades.

### Sumário

* [Assets](#assets)
* [Data](#data)
* [Messages](#messages)
* [Services](#services)
* [Views](#views)
* [ViewModels](#viewmodels)
* [App (Principal)](#app-principal)

---

### Assets

Contém os recursos estáticos utilizados na aplicação, como:

- Imagens
- Ícones
- Fontes
- Arquivos auxiliares usados pela interface gráfica.

---

### Data

Contém os dados, modelos e conversores utilizados pelo sistema.

- **Models:** define as entidades principais do domínio (ex.: agendas, tarefas, eventos, etc.).
- **Converters:** classes auxiliares responsáveis por conversão de dados para apresentação na interface, comumente utilizados em bindings Avalonia UI.

#### Converters

Os *Converters* são responsáveis por transformar e adaptar dados para apresentação na interface gráfica.

No padrão **MVVM com Avalonia UI**, os *Converters* permitem:

- Traduzir valores de propriedades em formatos adequados à interface (por exemplo: converter `bool` em `Visibility`).
- Adaptar enums, tipos ou objetos complexos para representação visual.
- Facilitar o binding entre os `ViewModels` e os controles gráficos.

##### DateConverter.cs

Realiza a conversão entre `DateTime` e `string` no formato `"dd/MM/yyyy"`, facilitando a exibição e entrada de datas no formato brasileiro.

- `Convert`: transforma um `DateTime` em string formatada.
- `ConvertBack`: transforma uma string formatada em `DateTime`.

```csharp
using System;
using System.Globalization;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;


public class DateConverter : IValueConverter
{
	public object? Convert(object? value,
	                       Type targetType,
	                       object? parameter,
	                       CultureInfo culture)
	{
		if (value is not DateTime date) return "";
		
		return date.ToString("dd/MM/yyyy");
	}

	public object ConvertBack(object? value,
	                           Type targetType,
	                           object? parameter,
	                           CultureInfo culture)
	{
		if (value is not string str) return DateTime.Now;
		
		return DateTime.ParseExact(str, "dd/MM/yyyy", CultureInfo.InvariantCulture);
	}
}
```

##### DateTimeOffSetConverter.cs

Converte entre `DateTime` e `DateTimeOffset`, permitindo que datas sejam manipuladas com informações de fuso horário quando necessário.

- `Convert`: de `DateTime` para `DateTimeOffset`.
- `ConvertBack`: de `DateTimeOffset` para `DateTime`.

```csharp
using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Agendai.Data.Converters;

public class DateTimeToDateTimeOffsetConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTime dateTime)
        {
            return new DateTimeOffset(dateTime);
        }
        return null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dto)
        {
            return dto.DateTime;
        }
        return DateTime.Now;
    }
}
```

##### RepeatsConverter.cs

Realiza a conversão entre o enum `Repeats` (definido em `Models`) e suas respectivas representações textuais em português.

- `Convert`: de `Repeats` para string (ex.: `Repeats.Daily` → `"Diariamente"`).
- `ConvertBack`: de string para `Repeats` (ex.: `"Mensalmente"` → `Repeats.Monthly`).

> Este converter não implementa a interface `IValueConverter`, sendo chamado de forma direta via código.

```csharp
using System;
using System.Globalization;
using Agendai.Data.Models;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;

public class RepeatsConverter
{
	public static string Convert(Repeats value)
	{
		return value switch
		{
			Repeats.None    => "Nunca",
			Repeats.Daily   => "Diariamente",
			Repeats.Weekly  => "Semanalmente",
			Repeats.Monthly => "Mensalmente",
			Repeats.Anually => "Anualmente",
			_               => ""
		};
	}

	public static Repeats ConvertBack(string value)
	{
		return value switch
		{
			"Nunca"        => Repeats.None,
			"Diariamente"  => Repeats.Daily,
			"Semanalmente" => Repeats.Weekly,
			"Mensalmente"  => Repeats.Monthly,
			"Anualmente"   => Repeats.Anually,
			_              => Repeats.None
		};
	}
}
```

##### StatusConverter.cs

Converte entre o enum `TodoStatus` (modelo de tarefas) e valores booleanos, para facilitar o binding em checkboxes e outros controles binários.

- `Convert`: transforma `TodoStatus` em `bool` (ex.: tarefas concluídas retornam `true`).
- `ConvertBack`: transforma `bool` em `TodoStatus` (ex.: `true` → `TodoStatus.Complete`).

```csharp
using System;
using System.Globalization;
using Agendai.Data.Models;
using Avalonia.Data.Converters;


namespace Agendai.Data.Converters;


public class StatusConverter : IValueConverter
{
	public object? Convert(object? value,
	                       Type targetType,
	                       object? parameter,
	                       CultureInfo culture)
	{
		if (value is not TodoStatus status) return false;

		return status switch
		{
			TodoStatus.Complete   => true,
			TodoStatus.Skipped    => true,
			TodoStatus.Incomplete => false,
			_ => throw new ArgumentOutOfRangeException(
				nameof(value),
				$"{nameof(value)} is not a valid {nameof(TodoStatus)}")
		};
	}

	public object? ConvertBack(object? value,
	                           Type targetType,
	                           object? parameter,
	                           CultureInfo culture)
	{
		if (value is not bool completed) return TodoStatus.Incomplete;

		return completed ? TodoStatus.Complete : TodoStatus.Incomplete;
	}
}
```

##### ViewIndexToTemplateConverter.cs

Converte um índice numérico (`int`) em um `DataTemplate` visual específico, permitindo selecionar dinamicamente o template correto de exibição conforme o modo de visualização do calendário.

- `Convert`: de índice `int` (0, 1, 2) para `DataTemplate` (mês, semana ou dia).
- `ConvertBack`: não implementado.

> Este converter permite alternar dinamicamente entre os modos de visualização do calendário na interface.

```csharp
using System;
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml.Templates;

namespace Agendai.Data.Converters;

public class ViewIndexToTemplateConverter : IValueConverter
{
    public DataTemplate? MonthTemplate { get; set; }
    public DataTemplate? WeekTemplate { get; set; }
    public DataTemplate? DayTemplate { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int index)
        {
            return index switch
            {
                0 => MonthTemplate,
                1 => WeekTemplate,
                2 => DayTemplate,
                _ => null
            };
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
        throw new NotImplementedException();
}
```

#### Models

Define as entidades principais do domínio (ex.: agendas, tarefas, eventos, etc.).  
Os modelos representam as estruturas de dados persistidas no banco de dados e utilizadas na lógica de negócios.

> Para a descrição completa dos modelos e suas propriedades, consulte a seção [Modelagem](#modelagem).

Arquivos adicionais nesta pasta:

- `EventsByAgenda.cs`
- `EventsByAgendaName.cs`
- `RepeatsOption.cs`
- `TodosByListName.cs`

Estes arquivos funcionam como estruturas auxiliares, DTOs (Data Transfer Objects) ou filtros de dados.

---

### Messages

Contém classes responsáveis pela comunicação interna da aplicação, facilitando a troca de mensagens entre os ViewModels.  
Este padrão contribui para o desacoplamento entre componentes da interface.

#### GetListsNamesMessenger.cs

Esta mensagem transporta uma lista de nomes de listas (`string[]`) através da propriedade `SelectedItemsName`.

- **Objetivo**: comunicar quais listas (ou categorias) foram selecionadas.
- **Uso típico**: atualizar filtros, dropdowns ou carregar tarefas associadas a listas específicas.

```csharp
public class GetListsNamesMessenger
{
    public string[] SelectedItemsName { get; }

    public GetListsNamesMessenger(string[] listNames)
    {
        SelectedItemsName = listNames;
    }
}
```

#### NavigateToDateMessenger.cs
Esta classe representa uma mensagem de navegação de data no sistema.

- **Propriedade:**
  - `SelectedDate (DateTime)`: contém a data selecionada que será utilizada para navegação.
- **Objetivo**: Permitir que, ao selecionar uma nova data, outros componentes da aplicação sejam notificados para ajustar a interface (ex.: mudar o foco do calendário, exibir eventos daquela data, etc).
- **Padrão aplicado**: segue o padrão de mensageria usado no MVVM para comunicação desacoplada entre `ViewModels`.

**Exemplo de estrutura:**

```csharp
public class NavigateToDateMessenger
{
    public DateTime SelectedDate { get; set; }

    public NavigateToDateMessenger(DateTime selectedDate)
    {
        SelectedDate = selectedDate;
    }
}
```
---

### Services

Contém as classes responsáveis pela lógica de negócio da aplicação e o processamento dos dados.

> Observação: Atualmente os serviços estão localizados na pasta `Services/views`, centralizando tanto regras de negócio quanto, possivelmente, alguns componentes relacionados à interface.

---

### Views

Contém as telas, páginas e componentes visuais da aplicação, implementados com Avalonia UI.  
Aqui estão definidos os layouts e controles apresentados ao usuário.

---

### ViewModels

Contém os **ViewModels**, que implementam o padrão **MVVM (Model-View-ViewModel)**.  
Os ViewModels fazem a mediação entre a interface e a lógica de negócios, mantendo o estado e comportamento das telas.

#### Main Window View Model
Consiste na view model de entrada do projeto, ela configura o que deve ser inicializado no projeto, implementando os métodos:

```csharp
namespace Agendai.ViewModels.Interfaces;

public interface IMainWindowViewModel
{
	public ViewModelBase? CurrentViewModel { get; set; }

	public void NavigateToHome();
	public void NavigateToAgenda();
	public void NavigateToTodo();
	public void NavigateToPomodoro();
}

```

#### View Model Base
View model que é implementada pelas outras para poder utilizar das funcionalidades de retornar para a home do projeto, implementando os métodos:

```csharp
using System.Windows.Input;


namespace Agendai.ViewModels.Interfaces;

public interface IViewModelBase
{
	public  MainWindowViewModel? MainViewModel     { get; set; }
	public  ICommand             ReturnHomeCommand { get; }
	private void                 ReturnHome()      { MainViewModel?.NavigateToHome(); }
}

```

#### Home Window View Model
View model da primeira tela do projeto, traz as principais funcionalidades de navegação, além disso, instancia as view models que precisam estar sendo utilizadas no resto do projeto

```csharp
using System.Windows.Input;


namespace Agendai.ViewModels.Interfaces;

public interface IHomeWindowViewModel
{
	public bool IsPopupOpen            { get; set; }
	public bool IsEventListsAbleToView { get; set; }
	public bool IsTodoListsAbleToView  { get; set; }
	public bool IsAgendaWindow         { get; set; }
	public bool IsTodoWindow           { get; set; }
	public bool IsPomodoroWindow       { set; }

	public TodoWindowViewModel? TodoWindowVm { get; set; }
	public EventListViewModel?  EventListVm  { get; set; }

	public string   EventListsVisibilityText { get; }
	public string   TodoListsVisibilityText  { get; }
	public ICommand OpenPopupCommand         { get; }
	public ICommand OpenTodoFormCommand      { get; }
	public ICommand OpenEventFormCommand     { get; }
	public ICommand OpenAgendaCommand        { get; }
	public ICommand OpenTodoCommand          { get; }
	public ICommand OpenPomodoroCommand      { get; }
}
```

#### Todo Window View Model
View model que traz todos os métodos gerais que tratam das tarefas, cuidando da criação, edição e visualização

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.ViewModels.Interfaces;

public interface ITodoWindowViewModel
{
	HomeWindowViewModel? HomeWindowVm { get; set; }
	EventListViewModel?  EventListVm  { get; set; }

	Action?   OnTaskAdded           { get; set; }
	ICommand? OpenPopupCommand      { get; }
	ICommand? SelectTarefaCommand   { get; }
	ICommand? AddTodoCommand        { get; }
	ICommand? CancelCommand         { get; }
	ICommand? AddTodoToEventCommand { get; }

	bool OpenAddTask { get; set; }

	ObservableCollection<Todo>          Todos            { get; set; }
	ObservableCollection<Todo>          IncompleteTodos  { get; set; }
	ObservableCollection<Todo>          TodoHistory      { get; set; }
	ObservableCollection<Todo>          IncompleteResume { get; set; }
	ObservableCollection<string>        ListNames        { get; set; }
	ObservableCollection<RepeatsOption> RepeatOptions    { get; }
	ObservableCollection<Todo?>         FreeTodos        { get; }
	IEnumerable<TodosByListName>        TodosByListName  { get; }
	IEnumerable<string>                 FreeTodosNames   { get; }

	string        NewTaskName      { get; set; }
	string        NewDescription   { get; set; }
	string        SelectedTodoName { get; set; }
	string        ListName         { get; set; }
	DateTime      NewDue           { get; set; }
	bool          IsPopupOpen      { get; set; }
	bool          HasChanges       { get; }
	RepeatsOption SelectedRepeats  { get; set; }
	Todo?         EditingTodo      { get; set; }
	Todo?         SelectedTodo     { get; set; }

	void ClearTodoForm();
}
```

#### Event List View Model
View model que trata as funcionalidades da parte de eventos, trazendo os métodos de criação, edição, visualização, relacionamentos de ventos

```csharp
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;
using Avalonia.Media;


namespace Agendai.ViewModels.Interfaces;

public interface IEventListViewModel
{
	public ObservableCollection<Event>         Events                { get; set; }
	public ObservableCollection<RepeatsOption> RepeatOptions         { get; }
	public ObservableCollection<string>        AgendaNames           { get; set; }
	public ObservableCollection<Todo?>         TodosForSelectedEvent { get; set; }
	public IEnumerable<EventsByAgenda>         EventsByAgendaName    { get; }

	public bool                 IsAddTodoPopupOpen { get; set; }
	public bool                 OpenAddEvent       { get; set; }
	public bool                 CanSave            { get; }
	public string               NewEventName       { get; set; }
	public DateTime             NewDue             { get; set; }
	public string               NewDescription     { get; set; }
	public string               AgendaName         { get; set; }
	public RepeatsOption        Repeat             { get; set; }
	public Event?               SelectedEvent      { get; }
	public bool                 HasRelatedTodos    { get; set; }
	public TodoWindowViewModel? TodoWindowVm       { get; }
	public Color?               NewColor           { get; set; }

	public Action?  OnEventAddedOrUpdated { get; set; }
	public ICommand AddEventCommand       { get; }
	public ICommand CancelCommand         { get; }
	public ICommand OpenPopupCommand      { get; }
	public ICommand ClosePopupCommand     { get; }

	public void LoadEvent(Event? ev);
	public void UpdateCanSave();
	public void NotifyTodosForSelectedEventChanged();
	public void RemoveTodoFromEvent(Todo? todo);
}
```

#### Agenda Window View Model
View model que cuida de todas as funcionalidades da agenda, visualizações (mensal, semanal e diária), mudança das mesmas, renderização das informações, tratamento de ações do usuário.

```csharp
using System;
using System.Collections.ObjectModel;
using Agendai.Data.Abstractions;
using Agendai.Data.Models;


namespace Agendai.ViewModels.Interfaces;

public interface IAgendaWindowViewModel
{
	public int      SelectedIndex     { get; set; }
	public string[] SelectedListNames { get; }
	public bool     ShowData          { get; }
	public string   SelectedMonth     { get; set; }
	public string   SelectedWeek      { get; set; }
	public string   SelectedDay       { get; set; }
	public DateTime CurrentMonth      { get; set; }
	public DateTime CurrentWeek       { get; set; }
	public DateTime CurrentDay        { get; set; }
	public string   SearchText        { get; set; }

	public ObservableCollection<string>   SearchableItems { get; set; }
	public ObservableCollection<MonthRow> MonthViewRows   { get; }
	public ObservableCollection<WeekRow>  WeekViewRows    { get; }
	public ObservableCollection<DayRow>   DayViewRows     { get; }

	public void GoToPreviousMonth();
	public void GoToNextMonth();
	public void GoToPreviousWeek();
	public void GoToNextWeek();
	public void GoToPreviousDay();
	public void GoToNextDay();
	public void GoToDay(int date);
	public void ToggleShowData();
	public void EditEvent(Event ev);
	public void EditTodo(Todo   todo);
	public void UpdateDataGridItems();
}
```

---

### App (Principal)

Contém a inicialização, configuração e ponto de entrada geral da aplicação:

- `App.axaml` e `App.axaml.cs` — configuração visual e global da aplicação Avalonia.
- `Program.cs` — ponto de entrada principal da aplicação.
- `ViewLocator.cs` — resolve dinamicamente as views associadas aos ViewModels.
- `app.manifest` — arquivo de manifesto e configurações do sistema.

---