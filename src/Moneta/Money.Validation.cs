namespace Bogoware.Moneta;

public partial class Money
{
	internal static decimal ValidateAndGetDecimalValue<T>(T value) where T : INumber<T>, IConvertible
	{
		try
		{
			return Convert.ToDecimal(value);
		}
		catch (InvalidCastException)
		{
			throw new InvalidCastException($"The type {typeof(T)} cannot be converted to decimal.");
		}
	}

	private static void ValidateWeights<T>(IEnumerable<T> weights) where T : INumber<T>
	{
		// all weights must be positive
		if (weights.Any(w => T.IsZero(w) || T.IsNegative(w)))
		{
			throw new ArgumentOutOfRangeException(nameof(weights), "All weights must be positive.");
		}
	}
}