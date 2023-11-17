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
	/// Verifies that the two currencies are compatible, i.e. they are either both the same
	/// or one of them is neutral.
	/// </summary>
	public static bool AreCompatible(ICurrency first, ICurrency second) => first.Equals(second);

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
}