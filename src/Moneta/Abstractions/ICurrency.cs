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
	/// A neutral currency is a currency that can be used in any operation with any second currency.
	/// </summary>
	bool IsNeutral { get; }

	/// <summary>
	/// Verifies that the two currencies are compatible, i.e. they are either both the same
	/// or one of them is neutral.
	/// </summary>
	public static bool AreCompatible(ICurrency first, ICurrency second) =>
		first.IsNeutral || second.IsNeutral || first.Equals(second);

	/// <summary>
	/// Ensures that the two currencies are compatible, i.e. they are either both the same
	/// or one of them is neutral. Throws a <see cref="CurrencyIncompatibleException"/> otherwise.
	/// </summary>
	/// <exception cref="CurrencyIncompatibleException"></exception>
	public static void MustBeCompatible(ICurrency first, ICurrency second)
	{
		if (!AreCompatible(first, second))
			throw new CurrencyIncompatibleException(first, second);
	}

	/// <summary>
	/// Determines the most specific currency between two currencies or throws a <see cref="CurrencyIncompatibleException"/>
	/// if the are not compatible.
	/// </summary>
	/// <exception cref="CurrencyIncompatibleException"></exception>
	public static void GetMostSpecificCurrency(
		ICurrency first, ICurrency second, out ICurrency lessSpecificCurrency, out ICurrency mostSpecificCurrency)
	{
		MustBeCompatible(first, second);

		// determine the most specific currency
		(mostSpecificCurrency, lessSpecificCurrency) = first.IsNeutral
			? (second, first)
			: (first, second);

		if (mostSpecificCurrency.IsNeutral
		    && lessSpecificCurrency.IsNeutral // for the sake of readability, redundant
		    && lessSpecificCurrency.DecimalPlaces > mostSpecificCurrency.DecimalPlaces) // both were neutral currencies
		{
			// chose the neutral currency with the most decimal places
			// swapping them
			(mostSpecificCurrency, lessSpecificCurrency) = (lessSpecificCurrency, mostSpecificCurrency);
		}
	}
}