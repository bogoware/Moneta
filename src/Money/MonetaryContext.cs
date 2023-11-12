namespace Bogoware.Money;

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
	/// The number of decimal places used for internal operations such as casting from <see cref="double"/> to the
	/// <see cref="Money.Amount"/> of the <see cref="Money"/> type or multiplication and division operations.
	/// </summary>
	public int OperationDecimalPlaces { get; }

	private List<ErrorRoundingOperation> InternalErrorRoundingOperations { get; }

	/// <summary>
	/// A list of residual amounts that have not been handled by the user.
	/// </summary>
	public IReadOnlyList<ErrorRoundingOperation> ErrorRoundingOperations => InternalErrorRoundingOperations;

	/// <summary>
	/// Signals that some rounding errors have occurred during operations without residual handling.
	/// </summary>
	public bool HasRoundingErrors => InternalErrorRoundingOperations.Count > 0;

	/// <summary>
	/// Initializes a new <see cref="MonetaryContext"/> instance.
	/// </summary>
	/// <param name="defaultCurrency"></param>
	/// <param name="rounding"></param>
	/// <param name="operationDecimalPlaces"><inheritdoc cref="OperationDecimalPlaces"/></param>
	public MonetaryContext(Currency defaultCurrency, MidpointRounding rounding, int operationDecimalPlaces = 8)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(operationDecimalPlaces, 4);
		DefaultCurrency = defaultCurrency;
		Rounding = rounding;
		InternalErrorRoundingOperations = new();
		OperationDecimalPlaces = operationDecimalPlaces;
	}

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// </summary>
	public Money NewMoney(decimal amount) => NewMoney(amount, DefaultCurrency);

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// </summary>
	public Money NewMoney(double amount) => NewMoney(amount, DefaultCurrency);

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>.
	/// </summary>
	public Money NewMoney(decimal amount, out decimal residue) => NewMoney(amount, DefaultCurrency, out residue);

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>.
	/// </summary>
	public Money NewMoney(double amount, out decimal residue) => NewMoney(amount, DefaultCurrency, out residue);

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount of the <see cref="Money"/> instance.</param>
	/// <param name="currency">The currency of the <see cref="Money"/> instance.</param>
	/// <param name="residue">The residual amount after rounding.</param>
	public Money NewMoney(decimal amount, Currency currency, out decimal residue)
	{
		var internalAmount = Math.Round(amount, OperationDecimalPlaces, Rounding);
		var newAmount = Math.Round(internalAmount, currency.DecimalPlaces, Rounding);
		residue = internalAmount - newAmount;
		return new(amount, currency, this);
	}

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// </summary>
	/// <param name="amount">The amount of the <see cref="Money"/> instance.</param>
	/// <param name="currency">The currency of the <see cref="Money"/> instance.</param>
	public Money NewMoney(decimal amount, Currency currency)
	{
		var result = NewMoney(amount, currency, out var residue);
		var errorRoundingOperation = new ConvertFromDoubleOperation(residue, currency);
		AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <param name="amount">The amount of the <see cref="Money"/> instance.</param>
	/// <param name="currency">The currency of the <see cref="Money"/> instance.</param>
	/// <param name="residue">The residual amount after rounding.</param>
	/// <returns></returns>
	public Money NewMoney(double amount, Currency currency, out decimal residue)
	{
		// ReSharper disable SuggestVarOrType_BuiltInTypes
		decimal approximatedAmount = Math.Round((decimal)amount, OperationDecimalPlaces, Rounding);
		decimal newAmount = Math.Round(approximatedAmount, currency.DecimalPlaces, Rounding);
		residue = approximatedAmount - newAmount;
		return new(newAmount, currency, this);
	}

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// </summary>
	/// <param name="amount">The amount of the <see cref="Money"/> instance.</param>
	/// <param name="currency">The currency of the <see cref="Money"/> instance.</param>
	public Money NewMoney(double amount, Currency currency)
	{
		var result = NewMoney(amount, currency, out var residue);
		var errorRoundingOperation = new ConvertFromDoubleOperation(residue, currency);
		AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}
	
	internal void AddErrorRoundingOperation(ErrorRoundingOperation errorRoundingOperation)
	{
		if (errorRoundingOperation.Residue == 0) return;
		InternalErrorRoundingOperations.Add(errorRoundingOperation);
	}

}