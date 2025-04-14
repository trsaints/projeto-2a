using System;
using System.Globalization;
using Agendai.Models;
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
