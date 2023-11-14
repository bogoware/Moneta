namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown in case of invalid configuration of the <see cref="MonetaContext"/>.
/// </summary>
public class MonetaryContextInvalidConfigurationException: Exception
{
	public MonetaryContextInvalidConfigurationException(string message) : base(message)
	{
	}
}