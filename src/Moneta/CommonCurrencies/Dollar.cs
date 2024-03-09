namespace Bogoware.Moneta.CommonCurrencies;

public class Dollar : ICurrency
{
	public string Code => "USD";
	public string Name => "Dollar";
	public string Symbol => "$";
	public int DecimalPlaces => 2;
	public static Dollar Instance { get; } = new();

	private Dollar()
	{
	}
}