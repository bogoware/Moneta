namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown when attempting to perform an operation on two
/// incompatible currencies.
/// </summary>
public class CurrencyIncompatibleException : InvalidOperationException
{
	public CurrencyIncompatibleException(Money lhs, Money rhs)
		: base($"Money '{lhs}' and '{rhs}' are not compatible.")
	{
	}
}