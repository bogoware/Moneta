namespace Bogoware.Moneta.CommonCurrencies;

public class PoundSterling: ICurrency
{
	public string Code => "GBP";
	public string Name => "Pound Sterling";
	public string Symbol => "£";
	public int DecimalPlaces => 2;
	public static PoundSterling Instance { get; } = new();

	private PoundSterling()
	{
	}
}