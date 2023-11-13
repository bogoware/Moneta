using System.Numerics;

namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Split the money into the specified number of parts.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="error">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding, out decimal error)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfParts);
		var internalAmount = Math.Round(Amount / numberOfParts, Context.RoundingErrorDecimals, rounding);
		var partAmount = Math.Round(internalAmount, Currency.DecimalPlaces, rounding);
		var partMoney = new Money(partAmount, Currency, Context);
		var parts = Enumerable.Repeat(partMoney, numberOfParts).ToList();
		error = (internalAmount - partAmount) * numberOfParts;
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="error">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, out decimal error) =>
		Split(numberOfParts, Context.RoundingMode, out error);

	/// <summary>
	/// Split the money into the specified number of parts.
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding)
	{
		var parts = Split(numberOfParts, rounding, out var residue);
		var errorRoundingOperation = new SplitOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts) => Split(numberOfParts, Context.RoundingMode);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="error">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding, out decimal error)
		where T : INumber<T>, IConvertible
	{
		// ReSharper disable PossibleMultipleEnumeration
		// ReSharper disable LoopCanBeConvertedToQuery
		ValidateType<T>();
		ValidateWeights(weights);

		var parts = new List<Money>(weights.Count());
		var totalWeight = T.Zero;
		foreach (var weight in weights)
		{
			totalWeight += weight;
		}

		var decimalTotalWeight = Convert.ToDecimal(totalWeight);

		error = 0;
		foreach (var weight in weights)
		{
			var decimalWeight = Convert.ToDecimal(weight);
			var proportion = decimalWeight / decimalTotalWeight;
			var amount = Math.Round(Amount * proportion, Currency.DecimalPlaces, rounding);
			error += -amount;
			parts.Add(new(amount, Currency, Context));
		}

		error += Amount;
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingErrorOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="error">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, out decimal error) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode, out error);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var parts = Split(weights, rounding, out var residue);
		var errorRoundingOperation = new SplitOperation(residue, Currency);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="residue">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode);
}