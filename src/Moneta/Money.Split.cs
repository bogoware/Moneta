namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Split the money into the specified number of parts.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingOperationError"/> to the <see cref="MonetaContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="unallocated">The residual part after the split. This value can be positive or negative depending by the rounding mode</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding, out Money unallocated)
	{
		ArgumentOutOfRangeException.ThrowIfNegativeOrZero(numberOfParts);
		var allocatedAmount = Math.Round(Amount / numberOfParts, Currency.DecimalPlaces, rounding);
		var allocatedMoney = new Money(allocatedAmount, Currency, Context);
		var parts = Enumerable.Repeat(allocatedMoney, numberOfParts).ToList();
		unallocated = new(Amount - numberOfParts * allocatedAmount, Currency, Context);
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingOperationError"/> to the <see cref="MonetaContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="unallocated">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, out Money unallocated) =>
		Split(numberOfParts, Context.RoundingMode, out unallocated);

	/// <summary>
	/// Split the money into the specified number of parts.
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts, MidpointRounding rounding)
	{
		var parts = Split(numberOfParts, rounding, out var unallocated);
		var errorRoundingOperation = new SplitOperationError(unallocated);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into the specified number of parts using the <see cref="Context"/>'s
	/// rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingOperationError"/> to the <see cref="MonetaContext"/>. 
	/// </summary>
	/// <param name="numberOfParts">The number of parts to split the money into.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split(int numberOfParts) => Split(numberOfParts, Context.RoundingMode);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingOperationError"/> to the <see cref="MonetaContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <param name="unallocated">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding, out Money unallocated)
		where T : INumber<T>, IConvertible
	{
		// ReSharper disable PossibleMultipleEnumeration
		// ReSharper disable LoopCanBeConvertedToQuery
		ValidateAndGetDecimalValue(T.Zero); // Validate T is convertible to decimal
		ValidateWeights(weights);

		var parts = new List<Money>(weights.Count());
		var totalWeight = T.Zero;
		foreach (var weight in weights)
		{
			totalWeight += weight;
		}

		var decimalTotalWeight = ValidateAndGetDecimalValue(totalWeight);

		decimal allocatedAmount = 0;
		foreach (var weight in weights)
		{
			var decimalWeight = Convert.ToDecimal(weight);
			var proportion = decimalWeight / decimalTotalWeight;
			var amount = Math.Round(Amount * proportion, Currency.DecimalPlaces, rounding);
			allocatedAmount += amount;
			parts.Add(new(amount, Currency, Context));
		}
		
		unallocated = new(Amount - allocatedAmount, Currency, Context);
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="RoundingOperationError"/> to the <see cref="MonetaContext"/>. 
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="unallocated">The residual part after the split. This value can be positive or negative.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, out Money unallocated) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode, out unallocated);

	/// <summary>
	/// Split the money into many parts using the specified weights.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <param name="rounding">The rounding mode to use.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights, MidpointRounding rounding) where T : INumber<T>, IConvertible
	{
		var parts = Split(weights, rounding, out var unallocated);
		var errorRoundingOperation = new SplitOperationError(unallocated);
		Context.AddRoundingErrorOperation(errorRoundingOperation);
		return parts;
	}

	/// <summary>
	/// Split the money into many parts using the specified weights and the <see cref="Context"/>'s rounding mode.
	/// </summary>
	/// <param name="weights">The weights to use for the split. All weights must be positive.</param>
	/// <returns>The list of parts.</returns>
	public List<Money> Split<T>(IEnumerable<T> weights) where T : INumber<T>, IConvertible =>
		Split(weights, Context.RoundingMode);
}