using System.Globalization;

namespace Bogoware.Money;

public partial class Money
{
	public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0} {1}", Currency, Amount);
}