using Bogoware.Moneta.Abstractions;

namespace Bogoware.Moneta.UnitTests.CurrencyTests;

public abstract class CurrencyBaseTests
{
	protected readonly ICurrency Usd = new Currency("USD", "US Dollar", "US Dollar", 2);
	protected readonly ICurrency Eur = new Currency("EUR", "Euro", "Euro", 2);
	protected readonly ICurrency Undefined = UndefinedCurrency.Instance;
}