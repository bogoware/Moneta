using System.Numerics;

namespace Bogoware.Money;

/// <summary>
/// A monetary context. 
/// </summary>
public sealed class MonetaryContext
{
	/// <summary>
	/// The default <see cref="Currency"/> for monetary operations
	/// </summary>
	public ICurrency DefaultCurrency { get; }

	/// <summary>
	/// The default roundingMode mode for monetary operations
	/// belonging to the context.
	/// </summary>
	public MidpointRounding RoundingMode { get; }

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
	/// Signals that some roundingMode errors have occurred during operations without residual handling.
	/// </summary>
	public bool HasRoundingErrors => InternalErrorRoundingOperations.Count > 0;

	/// <summary>
	/// Initializes a new <see cref="MonetaryContext"/> instance.
	/// </summary>
	/// <param name="defaultCurrency"></param>
	/// <param name="roundingMode"></param>
	/// <param name="operationDecimalPlaces"><inheritdoc cref="OperationDecimalPlaces"/></param>
	public MonetaryContext(
		ICurrency? defaultCurrency = default,
		MidpointRounding roundingMode = default,
		int operationDecimalPlaces = 8)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(operationDecimalPlaces, 4);
		DefaultCurrency = defaultCurrency ?? Currency.Undefined;
		RoundingMode = roundingMode;
		InternalErrorRoundingOperations = new();
		OperationDecimalPlaces = operationDecimalPlaces;
	}

	#region Money Factory Methods

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <returns></returns>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode, out decimal residue)
		where T : INumber<T>, IConvertible
	{
		// ReSharper disable SuggestVarOrType_BuiltInTypes
		Money.ValidateType<T>();
		decimal decimalAmount = Convert.ToDecimal(amount);
		decimal approximatedAmount = Math.Round(decimalAmount, OperationDecimalPlaces, roundingMode);
		decimal newAmount = Math.Round(approximatedAmount, currency.DecimalPlaces, roundingMode);
		residue = approximatedAmount - newAmount;
		return new(newAmount, currency, this);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency, out decimal residue) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, currency, RoundingMode, out residue);

	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, out decimal residue) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, DefaultCurrency, out residue);


	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// In case of roundingMode errors, the residual part is added to the <see cref="MonetaryContext"/> as a
	/// </summary>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode)
		where T : INumber<T>, IConvertible
	{
		var result = CreateMoney(amount, currency, roundingMode, out var residue);
		var errorRoundingOperation = new CreateOperation(residue, currency);
		AddErrorRoundingOperation(errorRoundingOperation);
		return result;
	}

	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, currency, RoundingMode);
	
	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding)"/>
	public Money CreateMoney<T>(T amount) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, DefaultCurrency, RoundingMode);

	#endregion Money Factory Methods

	/// <summary>
	/// Returns true if the specified <see cref="Money"/> instance belongs to the <see cref="MonetaryContext"/>.
	/// </summary>
	public bool Owns(Money money) => money.Context == this;

	internal void AddErrorRoundingOperation(ErrorRoundingOperation errorRoundingOperation)
	{
		if (errorRoundingOperation.Residue == 0) return;
		InternalErrorRoundingOperations.Add(errorRoundingOperation);
	}
}