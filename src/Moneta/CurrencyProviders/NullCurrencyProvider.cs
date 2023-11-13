using System.Diagnostics.CodeAnalysis;
using Bogoware.Moneta.Abstractions;

namespace Bogoware.Moneta.CurrencyProviders;

/// <summary>
/// A currency provider that holds no currencies.
/// </summary>
public class NullCurrencyProvider: ICurrencyProvider
{
	public bool TryGetCurrency(string code, [NotNullWhen(true)] out ICurrency? currency)
	{
		currency = null;
		return false;
	}
}