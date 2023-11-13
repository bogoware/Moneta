namespace Bogoware.Moneta.Abstractions;

/// <summary>
/// Currency interface.
/// </summary>
public interface ICurrency
{
	/// <summary>
	/// Should be the ISO 4217 currency code.
	/// </summary>
	string Code { get; }

	/// <summary>
	/// The international name of the currency.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// The international symbol of the currency.
	/// </summary>
	string Symbol { get; }

	/// <summary>
	/// The number of decimal places used by the currency.
	/// </summary>
	int DecimalPlaces { get; }

	/// <summary>
	/// A neutral currency is a currency that can be used in any operation with any other currency.
	/// </summary>
	bool IsNeutral { get; }
}