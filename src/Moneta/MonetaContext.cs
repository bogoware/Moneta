using Bogoware.Moneta.CurrencyProviders;

namespace Bogoware.Moneta;

/// <summary>
/// A monetary context. 
/// </summary>
public sealed class MonetaContext: IDisposable
{
	/// <summary>
	/// The default number of decimals used for rounding errors detection.
	/// <seealso cref="RoundingErrorDecimals"/>
	/// </summary>
	public const int DefaultRoundingErrorDecimals = 8;

	/// <summary>
	/// The default <see cref="Currency"/> for new money instances.
	/// </summary>
	public ICurrency DefaultCurrency { get; }

	/// <summary>
	/// The <see cref="ICurrencyProvider"/> used to retrieve currencies.
	/// </summary>
	public ICurrencyProvider CurrencyProvider { get; }

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

	private List<RoundingOperationError> InternalRoundingErrors { get; }

	/// <summary>
	/// The rounding errors occurred during operations performed without rounding error handling.
	/// </summary>
	public IReadOnlyList<RoundingOperationError> RoundingErrors => InternalRoundingErrors;

	/// <summary>
	/// The context has rounding errors.
	/// </summary>
	public bool HasRoundingErrors => InternalRoundingErrors.Count > 0;

	/// <summary>
	/// Initializes a new <see cref="MonetaContext"/> instance.
	/// </summary>
	/// <param name="defaultCurrency">The currency used to create new money. Default: <see cref="DefaultCurrency"/>.</param>
	/// <param name="currencyProvider">The currency provider used to retrieve currencies. Default: <see cref="NullCurrencyProvider"/>.</param>
	/// <param name="roundingMode">The rounding mode used for monetary operations and internal error rounding detection. Default: <see cref="MidpointRounding.ToEven"/>.</param>
	/// <param name="roundingErrorDecimals">The number of decimal places used for error rounding detection, default is <see cref="DefaultRoundingErrorDecimals"/>.</param>
	public MonetaContext(
		ICurrency? defaultCurrency = default,
		ICurrencyProvider? currencyProvider = default,
		MidpointRounding roundingMode = default,
		int roundingErrorDecimals = DefaultRoundingErrorDecimals)
	{
		ArgumentOutOfRangeException.ThrowIfLessThan(roundingErrorDecimals, 4);
		DefaultCurrency = defaultCurrency ?? UndefinedCurrency.Instance;
		CurrencyProvider = currencyProvider ?? new NullCurrencyProvider();
		RoundingMode = roundingMode;
		InternalRoundingErrors = new();
		RoundingErrorDecimals = roundingErrorDecimals;
	}

	/// <inheritdoc cref="MonetaContext(Bogoware.Moneta.Abstractions.ICurrency?,Bogoware.Moneta.Abstractions.ICurrencyProvider?,System.MidpointRounding,int)"/>
	public MonetaContext(
		string defaultCurrency,
		ICurrencyProvider currencyProvider,
		MidpointRounding roundingMode = default,
		int roundingErrorDecimals = DefaultRoundingErrorDecimals)
		: this(currencyProvider.GetCurrency(defaultCurrency), currencyProvider, roundingMode, roundingErrorDecimals)
	{
	}

	#region Money Factory Methods (Safe ones)

	/// <summary>
	/// Initializes a new <see cref="Money"/> instance with the specified currency.
	/// </summary>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode, out decimal error)
		where T : INumber<T>, IConvertible
	{
		if (currency.DecimalPlaces > RoundingErrorDecimals)
			throw new MonetaryContextInvalidConfigurationException(
				$"The currency {currency.Code} has more decimal places ({currency.DecimalPlaces}) than the rounding error decimals ({RoundingErrorDecimals}).");
		
		var decimalAmount = Money.ValidateAndGetDecimalValue(amount);
		var approximatedAmount = Math.Round(decimalAmount, RoundingErrorDecimals, roundingMode);
		var newAmount = Math.Round(approximatedAmount, currency.DecimalPlaces, roundingMode);
		error = approximatedAmount - newAmount;
		return new(newAmount, currency, this);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	/// <exception cref="CurrencyNotFoundException">Thrown when the currency is not found.</exception>
	public Money CreateMoney<T>(T amount, string currency, MidpointRounding roundingMode, out decimal error)
		where T : INumber<T>, IConvertible
	{
		var currencyInstance = CurrencyProvider.GetCurrency(currency);
		return CreateMoney(amount, currencyInstance, roundingMode, out error);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency, out decimal error) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, currency, RoundingMode, out error);

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	/// <exception cref="CurrencyNotFoundException">Thrown when the currency is not found.</exception>
	public Money CreateMoney<T>(T amount, string currency, out decimal error) where T : INumber<T>, IConvertible
	{
		var currencyInstance = CurrencyProvider.GetCurrency(currency);
		return CreateMoney(amount, currencyInstance, RoundingMode, out error);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, out decimal error) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, DefaultCurrency, RoundingMode, out error);

	#endregion Money Factory Methods (Safe ones)

	#region Money Factory Methods (Unsafe ones)

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency, MidpointRounding roundingMode)
		where T : INumber<T>, IConvertible
	{
		var result = CreateMoney(amount, currency, roundingMode, out var error);
		var errorRoundingOperation = new CreateOperationError(error, currency);
		AddRoundingErrorOperation(errorRoundingOperation);
		return result;
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	/// <exception cref="CurrencyNotFoundException">Thrown when the currency is not found.</exception>
	public Money CreateMoney<T>(T amount, string currency, MidpointRounding roundingMode)
		where T : INumber<T>, IConvertible
	{
		var currencyInstance = CurrencyProvider.GetCurrency(currency);
		return CreateMoney(amount, currencyInstance, roundingMode);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding)"/>
	public Money CreateMoney<T>(T amount, ICurrency currency) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, currency, RoundingMode);

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding,out decimal)"/>
	/// <exception cref="CurrencyNotFoundException">Thrown when the currency is not found.</exception>
	public Money CreateMoney<T>(T amount, string currency) where T : INumber<T>, IConvertible
	{
		var currencyInstance = CurrencyProvider.GetCurrency(currency);
		return CreateMoney(amount, currencyInstance, RoundingMode);
	}

	/// <inheritdoc cref="CreateMoney{T}(T,ICurrency,System.MidpointRounding)"/>
	public Money CreateMoney<T>(T amount) where T : INumber<T>, IConvertible =>
		CreateMoney(amount, DefaultCurrency, RoundingMode);

	#endregion Money Factory Methods (Unsafe ones)

	/// <summary>
	/// Returns true if the specified <see cref="Money"/> instance belongs to the <see cref="MonetaContext"/>.
	/// </summary>
	public bool Owns(Money money) => money.Context == this;
	
	/// <summary>
	/// Throws an exception if the context has rounding errors.
	/// </summary>
	/// <exception cref="MonetaryContextWithRoundingErrorsException"></exception>
	public void EnsureNoRoundingErrors()
	{
		if (HasRoundingErrors)
			throw new MonetaryContextWithRoundingErrorsException(InternalRoundingErrors);
	}
	
	/// <summary>
	/// Clears the rounding errors.
	/// </summary>
	public void ClearRoundingErrors() => InternalRoundingErrors.Clear();

	internal void AddRoundingErrorOperation(RoundingOperationError roundingOperationError)
	{
		if (roundingOperationError.Error == 0) return;
		InternalRoundingErrors.Add(roundingOperationError);
	}
	
	public void Dispose() => EnsureNoRoundingErrors();
}