using System.Numerics;

namespace Bogoware.Money;

public partial class Money
{
	/// <summary>
	/// Map the specified functor to the money and return the result.
	/// </summary>
	/// <param name="functor"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public Money Map<T>(Func<Money, T> functor) where T : INumber<T>, IConvertible
	{
		var result = Apply(functor, out var error);
		Context.AddErrorRoundingOperation(new MapOperation(error, Currency));
		return result;
	}

	/// <inheritdoc cref="Map{T}(System.Func{Bogoware.Money.Money,T})"/>
	public Money Map<T>(Func<decimal, T> functor) where T : INumber<T>, IConvertible
	{
		var result = Apply(functor, out var error);
		Context.AddErrorRoundingOperation(new MapOperation(error, Currency));
		return result;
	}
}