using Bogoware.Moneta;
using Bogoware.Moneta.CommonCurrencies;

namespace MonetaHelloWorld;

public static class HelloWorldMoneta
{
	public static void BadCode()
	{
		using var moneta = new MonetaContext();
		var unitPrice = moneta.Dollar(1.12m);
		var quantity = 12.43424m;
		var finalPrice = unitPrice * quantity;
		
		Console.WriteLine($"Unit price: {unitPrice}");
		Console.WriteLine($"Quantity: {quantity}");
		Console.WriteLine($"Final price: {finalPrice}");
		
	} // an exception will be thrown because rounding errors occurred and were not handled
	
	public static void GoodCode()
	{
		using var moneta = new MonetaContext();
		var unitPrice = moneta.Dollar(1.12m);
		var quantity = 12.43424m;
		var finalPrice = unitPrice * quantity;
		
		Console.WriteLine($"Unit price: {unitPrice}");
		Console.WriteLine($"Quantity: {quantity}");
		Console.WriteLine($"Final price: {finalPrice}");

		if (moneta.HasRoundingErrors)
		{
			// Handle rounding errors as you prefer
			moneta.ClearRoundingErrors();
		}
	}
}