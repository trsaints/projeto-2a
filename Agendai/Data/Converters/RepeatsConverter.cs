using Agendai.Data.Models;


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
}
