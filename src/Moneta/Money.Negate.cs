namespace Bogoware.Moneta;

public partial class Money
{
	/// <summary>
	/// Returns a new <see cref="Money"/> object with the same amount but the opposite sign.
	/// </summary>
	public Money Negate() => new(-Amount, Currency, Context);
	
	public static Money operator -(Money money) => money.Negate();
}