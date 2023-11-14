namespace Bogoware.Moneta.Exceptions;

/// <summary>
/// Exception thrown when a monetary operation results in rounding errors.
/// </summary>
public class MonetaryContextWithRoundingErrorsException : Exception
{
	public IReadOnlyList<RoundingErrorOperation> RoundingErrors { get; }
	public MonetaryContextWithRoundingErrorsException(IEnumerable<RoundingErrorOperation> roundingErrors) : base()
	{
		RoundingErrors = roundingErrors.ToList();
	}
}