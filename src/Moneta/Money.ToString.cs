using System.Globalization;

namespace Bogoware.Moneta;

public partial class Money
{
	
	public override string ToString()
	{
		var format = $"{{0}} {{1:F{Currency.DecimalPlaces}}}";
		return string.Format(CultureInfo.InvariantCulture, format, Currency, Amount);
	}
}