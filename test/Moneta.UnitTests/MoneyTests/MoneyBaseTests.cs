using Bogoware.Moneta.Abstractions;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

/// <summary>
/// Money base test class
/// </summary>
public abstract class MoneyBaseTests
{
	// ReSharper disable RedundantArgumentDefaultValue
	protected readonly ICurrency Euro = new Currency("EUR", "Euro", "€", 2);
	protected readonly ICurrency UsDollar = new Currency("USD", "US Dollar", "$", 2);
}