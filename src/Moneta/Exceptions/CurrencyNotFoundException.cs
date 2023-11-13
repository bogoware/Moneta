namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown when a currency is not found.
/// </summary>
public class CurrencyNotFoundException : Exception
{
	public string Code { get; }
	public CurrencyNotFoundException(string code)
		: base($"Currency with code '{code}' was not found.")
	{
		Code = code;
	}
}