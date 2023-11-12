namespace Bogoware.Money;

public abstract record ErrorRoundingOperation;

public record DivideOperation(Money Residue) : ErrorRoundingOperation;

public record ConvertFromDoubleOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation;

/// <summary>
/// A monetary context. 
/// </summary>
public sealed class MonetaryContext
{
	/// <summary>
	/// The default <see cref="Currency"/> for monetary operations
	/// </summary>
	public Currency DefaultCurrency { get; }

	/// <summary>
	/// The default rounding mode for monetary operations
	/// belonging to the context.
	/// </summary>
	public MidpointRounding Rounding { get; }

	/// <summary>
	/// The number of decimal places to use to detect an error casting from <see cref="double"/> to the
	/// <see cref="Money.Amount"/> of the <see cref="Money"/> type.
	/// </summary>
	public int CastingErrorDetectionDecimalPlaces { get; }

	
	private List<ErrorRoundingOperation> InternalErrorRoundingOperations { get; }
	/// <summary>
	/// A list of residual amounts that have not been handled by the user.
	/// </summary>
	public IReadOnlyList<ErrorRoundingOperation> ErrorRoundingOperations => InternalErrorRoundingOperations;

	/// <summary>
	/// Signals that some rounding errors have occurred during operations without residual handling.
	/// </summary>
	public bool HasRoundingErrors => InternalErrorRoundingOperations.Count > 0;

	public MonetaryContext(
		Currency defaultCurrency, MidpointRounding rounding, int castingErrorDetectionDecimalPlaces = 8)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(castingErrorDetectionDecimalPlaces, 4);
		DefaultCurrency = defaultCurrency;
		Rounding = rounding;
		InternalErrorRoundingOperations = new();
		CastingErrorDetectionDecimalPlaces = castingErrorDetectionDecimalPlaces;
	}

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// </summary>
	public Money NewMoney(decimal amount) => NewMoney(amount, DefaultCurrency);
	public Money NewMoney(double amount) => NewMoney(amount, DefaultCurrency);

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// </summary>
	public Money NewMoney(decimal amount, Currency currency) => new(amount, currency, this);
	public Money NewMoney(double amount, Currency currency)
	{
		// ReSharper disable SuggestVarOrType_BuiltInTypes
		decimal approximatedAmount = Math.Round((decimal)amount, CastingErrorDetectionDecimalPlaces, Rounding);
		decimal newAmount = Math.Round(approximatedAmount, currency.DecimalPlaces, Rounding);
		AddConversionErrorRounding(approximatedAmount - newAmount, currency);
		return new(newAmount, currency, this);
	}

	internal void AddDivisionErrorRounding(Money money)
	{
		if (money.Amount == 0) return;
		InternalErrorRoundingOperations.Add(new DivideOperation(money));
	}

	internal void AddConversionErrorRounding(decimal residue, Currency currency)
	{
		if (residue == 0) return;
		InternalErrorRoundingOperations.Add(new ConvertFromDoubleOperation(residue, currency));
	}
}