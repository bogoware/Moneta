namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown in case of invalid configuration of the <see cref="MonetaryContext"/>.
/// </summary>
public class MonetaryContextInvalidConfigurationException: Exception
{
	public MonetaryContextInvalidConfigurationException(string message) : base(message)
	{
	}
}