namespace Bogoware.Moneta.CommonCurrencies;

using DollarCurrency = Dollar;
using EuroCurrency = Euro;
using YuanCurrency = Yuan;
using YenCurrency = Yen;
using PoundSterlingCurrency = PoundSterling;

public static class MonetaContextExtensions
{
	/// <summary>
	/// Creates a new <see cref="Money"/> instance with the specified amount and the <see cref="DollarCurrency"/> currency.
	/// </summary>
	/// <returns></returns>
	public static Money Dollar<T>(this MonetaContext context, T amount) where T : INumber<T>, IConvertible =>
		context.CreateMoney(amount, DollarCurrency.Instance);
	
	/// <summary>
	/// Creates a new <see cref="Money"/> instance with the specified amount and the <see cref="EuroCurrency"/> currency.
	/// </summary>
	public static Money Euro<T>(this MonetaContext context, T amount) where T : INumber<T>, IConvertible =>
		context.CreateMoney(amount, EuroCurrency.Instance);
	
	/// <summary>
	/// Creates a new <see cref="Money"/> instance with the specified amount and the <see cref="YuanCurrency"/> currency.
	/// </summary>
	public static Money Yuan<T>(this MonetaContext context, T amount) where T : INumber<T>, IConvertible =>
		context.CreateMoney(amount, YuanCurrency.Instance);
	
	/// <summary>
	/// Creates a new <see cref="Money"/> instance with the specified amount and the <see cref="YenCurrency"/> currency.
	/// </summary>
	public static Money Yen<T>(this MonetaContext context, T amount) where T : INumber<T>, IConvertible =>
		context.CreateMoney(amount, YenCurrency.Instance);
	
	/// <summary>
	/// Creates a new <see cref="Money"/> instance with the specified amount and the <see cref="PoundSterlingCurrency"/> currency.
	/// </summary>
	public static Money PoundSterling<T>(this MonetaContext context, T amount) where T : INumber<T>, IConvertible =>
		context.CreateMoney(amount, PoundSterlingCurrency.Instance);
	
}