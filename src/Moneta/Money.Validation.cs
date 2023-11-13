using System.Numerics;
using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta;

public partial class Money
{
	internal static void ValidateType<T>() where T : INumber<T>, IConvertible
	{
		try
		{
			decimal d = Convert.ToDecimal(T.Zero);
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

	private static void ValidateOperands(Money lhs, Money rhs)
	{
		if (lhs.Currency.IsNeutral
		    || rhs.Currency.IsNeutral
		    || lhs.Currency == rhs.Currency) return;

		throw new CurrencyIncompatibleException(lhs, rhs);
	}
}