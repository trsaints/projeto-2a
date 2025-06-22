using Agendai.Data.Converters;
using Agendai.Data.Models;


namespace Agendai.Data.Abstractions;

public class RepeatsOption
{
	public Repeats Repeats     { get; set; }
	public string  DisplayText => RepeatsConverter.Convert(Repeats);

	public override string ToString() { return DisplayText; }
}
