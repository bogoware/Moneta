using System.Numerics;

namespace Bogoware.Money;

/// <summary>
/// A monetary context. 
/// </summary>
public sealed class MonetaryContext
{
	/// <summary>
	/// The default number of decimals used for rounding errors detection.
	/// <seealso cref="RoundingErrorDecimals"/>
	/// </summary>
	public const int DefaultRoundingErrorDecimals = 8;
	/// <summary>
	/// The default <see cref="Currency"/> for monetary operations
	/// </summary>
	public ICurrency DefaultCurrency { get; }

	/// <summary>
	/// The default roundingMode mode for monetary operations and internal
	/// error rounding detection.
	/// </summary>
	public MidpointRounding RoundingMode { get; }

	/// <summary>
	/// The number of decimal places used for error rounding detection, default is <see cref="DefaultRoundingErrorDecimals"/>.
	/// Floating point types are converted to <see cref="decimal"/> with the specified number of decimals
	/// and then used for monetary operations.
	/// If this parameter is less then or equal to currency's <see cref="ICurrency.DecimalPlaces"/>, then
	/// no rounding errors can be detected.
	/// </summary>
	public int RoundingErrorDecimals { get; }

	private List<ErrorRoundingOperation> InternalRoundingErrors { get; }

	/// <summary>
	/// The rounding errors occurred during operations performed without rounding error handling.
	/// </summary>
	public IReadOnlyList<ErrorRoundingOperation> RoundingErrors => InternalRoundingErrors;

	/// <summary>
	/// The context has rounding errors.
	/// </summary>
	public bool HasRoundingErrors => InternalRoundingErrors.Count > 0;

	/// <summary>
	/// Initializes a new <see cref="MonetaryContext"/> instance.
	/// </summary>
	public MonetaryContext(
		ICurrency? defaultCurrency = default,
		MidpointRounding roundingMode = default,
		int roundingErrorDecimals = DefaultRoundingErrorDecimals)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(roundingErrorDecimals, 4);
		DefaultCurrency = defaultCurrency ?? Currency.Undefined;
		RoundingMode = roundingMode;
		InternalRoundingErrors = new();
		RoundingErrorDecimals = roundingErrorDecimals;
	}

	#region Money Factory Methods

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// This operation assume that the caller will handle properly the residual part
	/// and therefore does not add a <see cref="ErrorRoundingOperation"/> to the <see cref="MonetaryContext"/>. 
	/// </summary>
	/// <returns></returns>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode, out decimal error)
		where T : INumber<T>, IConvertible
	{
		// ReSharper disable SuggestVarOrType_BuiltInTypes
		Money.ValidateType<T>();
		decimal decimalAmount = Convert.ToDecimal(amount);
		decimal approximatedAmount = Math.Round(decimalAmount, RoundingErrorDecimals, roundingMode);
		decimal newAmount = Math.Round(approximatedAmount, currency.DecimalPlaces, roundingMode);
		error = approximatedAmount - newAmount;
		return new(newAmount, currency, this);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency, out decimal error) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, currency, RoundingMode, out error);

	/// <inheritdoc cref="CreateMoney{T}(T,Bogoware.Money.ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, out decimal error) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, DefaultCurrency, out error);


	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the default currency.
	/// In case of roundingMode errors, the residual part is added to the <see cref="MonetaryContext"/> as a
	/// </summary>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode)
		where T : INumber<T>, IConvertible
	{
		var result = CreateMoney(amount, currency, roundingMode, out var error);
		var errorRoundingOperation = new CreateOperation(error, currency);
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
		InternalRoundingErrors.Add(errorRoundingOperation);
	}
}