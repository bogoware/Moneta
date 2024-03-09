namespace Bogoware.Moneta.CommonCurrencies;

public class Euro: ICurrency
{
	public string Code => "EUR";
	public string Name => "Euro";
	public string Symbol => "€";
	public int DecimalPlaces => 2;
	public static Euro Instance { get; } = new();
	private Euro() { }
}