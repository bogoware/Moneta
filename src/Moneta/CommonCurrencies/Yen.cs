namespace Bogoware.Moneta.CommonCurrencies;

public class Yen: ICurrency
{
	public string Code => "JPY";
	public string Name => "Yen";
	public string Symbol => "¥";
	public int DecimalPlaces => 0;
	public static Yen Instance { get; } = new();

	private Yen()
	{
	}
}