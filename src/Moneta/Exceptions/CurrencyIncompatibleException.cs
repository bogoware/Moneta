namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown when attempting to perform an operation on two
/// incompatible currencies.
/// </summary>
public class CurrencyIncompatibleException : InvalidOperationException
{
	public CurrencyIncompatibleException(ICurrency lhs, ICurrency rhs)
		: base($"Currencies '{lhs}' and '{rhs}' are not compatible.")
	{
	}
}