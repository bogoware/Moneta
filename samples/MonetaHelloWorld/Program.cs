using Bogoware.Moneta;
using Bogoware.Moneta.CurrencyProviders;

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