using Bogoware.Moneta;
using Bogoware.Moneta.CurrencyProviders;
using MonetaHelloWorld;

var currency = new Currency("BITCOIN", "Bitcoin", "B", 8);
using (var context = new MonetaContext(currency))
{
};

using (var context = new MonetaContext("EUR", new IsoCurrencyProvider()))
{
	Console.WriteLine("Sample 1: no rounding errors occurred");
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;

	Console.WriteLine($"The final amount is {money}");
} // OK!

using (var context = new MonetaContext("USD", new IsoCurrencyProvider()))
{
	Console.WriteLine("\nSample 2: rounding errors occurred but were handled");
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;
	money += 1.2321; // Unhandled Rounding error

	if (context.HasRoundingErrors)
	{
		Console.WriteLine(" > Rounding errors detected");
		foreach (var error in context.RoundingErrors)
		{
			// TODO: Handle rounding errors
			Console.WriteLine($"   Error: {error}");
		}

		context.ClearRoundingErrors();
	}

	Console.WriteLine($"The final amount is {money}");
} // OK!

try
{
	using var context = new MonetaContext("USD", new IsoCurrencyProvider());
	Console.WriteLine("\nSample 3: rounding errors occurred but were not handled");
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;
	money += 1.2321; // Unhandled Rounding error

	Console.WriteLine($"The final amount is {money}");
} // KO! Rounding error not handled
catch (Exception ex)
{
	Console.WriteLine(ex);
}

// Sample 4: weighted split with unallocated money and rounding error
using (var context = new MonetaContext("EUR", new IsoCurrencyProvider()))
{
	Console.WriteLine("\nSample 4: weighted split with unallocated money and rounding error");
	var money = context.CreateMoney(11.11);
	var weights = Enumerable.Repeat(0.333333, 3);

	var split = money.Split(weights, out var unallocated);

	Console.WriteLine($"The original amount is {money}");
	Console.WriteLine($"The allocated amounts are: {string.Join(", ", split)}");
	Console.WriteLine($"The unallocated amount is {unallocated}");
} // OK!

// Sample 5: Rounding the final amount to the nearest 0.05 EUR (Cash rounding)
using (var context = new MonetaContext("EUR", new IsoCurrencyProvider()))
{
	Console.WriteLine("\nSample 5: Rounding the final amount to the nearest 0.05 EUR (Cash rounding)");
	var amounts = Enumerable.Repeat(context.CreateMoney(3.37), 17);

	var total = amounts.Aggregate((x, y) => x + y);  // sum up all the amounts
	var cashUnit = context.CreateMoney(0.05); // define the cash unit
	// round off the total to the highest multiple of the cash unit that is less than or equal to the total
	// a kindness to our customers that always save some pennies :)
	var cashTotal = total.RoundOff(cashUnit, MidpointRounding.ToZero, out var unallocated);

	Console.WriteLine($"The original total amount is {total}");
	Console.WriteLine($"The cash total amount is {cashTotal}");
	Console.WriteLine($"The discounted amount is {unallocated}");
} // OK!

// Sample 6: Calculating the P/E Ratio
using (var context = new MonetaContext("USD", new IsoCurrencyProvider()))
{
	Console.WriteLine("\nSample 6: Calculating the P/E Ratio");
	var price = context.CreateMoney(100);
	var earnings = context.CreateMoney(10);

	var peRatio = price / earnings;

	Console.WriteLine($"The P/E Ratio is {peRatio}");
} // OK!