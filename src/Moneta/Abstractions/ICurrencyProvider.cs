using System.Diagnostics.CodeAnalysis;

namespace Bogoware.Moneta.Abstractions;

/// <summary>
/// A currency provider is a service that provides <see cref="ICurrency"/> instances.
/// </summary>
public interface ICurrencyProvider
{
	/// <summary>
	/// Retrieves a currency by its code.
	/// </summary>
	/// <exception cref="CurrencyNotFoundException"></exception>
	ICurrency GetCurrency(string code) => TryGetCurrency(code, out var currency)
		? currency
		: throw new CurrencyNotFoundException(code);
	
	/// <summary>
	/// Try to retrieve a currency by its code.
	/// </summary>
	bool TryGetCurrency(string code, [NotNullWhen(true)] out ICurrency? currency);
}