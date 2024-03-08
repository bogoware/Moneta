namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown when a monetary operation results in rounding errors.
/// </summary>
public class MonetaryContextWithRoundingErrorsException : Exception
{
	public IReadOnlyList<RoundingOperationError> RoundingErrors { get; }
	public MonetaryContextWithRoundingErrorsException(IEnumerable<RoundingOperationError> roundingErrors) : base()
	{
		RoundingErrors = roundingErrors.ToList();
	}
}