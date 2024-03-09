namespace Bogoware.Moneta.CommonCurrencies;

public class Yuan: ICurrency
{
	public string Code => "CNY";
	public string Name => "Yuan";
	public string Symbol => "¥";
	public int DecimalPlaces => 2;
	public static Yuan Instance { get; } = new();

	private Yuan()
	{
	}
}