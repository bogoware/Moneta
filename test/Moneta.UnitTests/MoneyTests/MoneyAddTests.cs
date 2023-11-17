
// ReSharper disable RedundantArgumentDefaultValue

using Bogoware.Moneta.Exceptions;
using static System.MidpointRounding;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyAddTests : MoneyBaseTests
{
	[Fact]
	public void Adding_cant_create_value()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: ToPositiveInfinity);
		var sut = context.CreateMoney(1.00M);
		var other = 0.001M;
		
		// Act
		var result = sut.Add(other, out var error);
		
		// Arrange
		result.Amount.Should().Be(1.01M);
		error.Should().Be(-0.009M);
	}
	
	[Fact]
	public void Adding_cant_destroy_value()
	{
		// Arrange
		var context = new MonetaContext(Euro, roundingMode: ToNegativeInfinity);
		var sut = context.CreateMoney(1.00M);
		var other = 0.001M;
		
		// Act
		var result = sut.Add(other, out var error);
		
		// Arrange
		result.Amount.Should().Be(1.00M);
		error.Should().Be(0.001M);
	}
	
	[Fact]
	public void Adding_incompatibleMoney_throwsException()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00M, Euro);
		var other = context.CreateMoney(1.00M, UsDollar);
		
		// Act and Assert
		sut.Invoking(x => x.Add(other))
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Currencies 'EUR' and 'USD' are not compatible.");
	}
	
	[Fact]
	public void Adding_withError_isSafe()
	{
		// Arrange
		var context = new MonetaContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1.00M);
		const double amount = 0.12345;

		// Act
		var result = sut.Add(amount, out var error);

		// Assert
		result.Amount.Should().Be(1.12M);
		error.Should().Be(0.00345M);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Adding_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1.00M);
		var amount = 0.12345;

		// Act
		var result = sut.Add(amount);

		// Assert
		result.Amount.Should().Be(1.12M);
		context.HasRoundingErrors.Should().BeTrue();
		context.RoundingErrors.Should().HaveCount(1);
		var error = context.RoundingErrors.First();
		error.Should().BeOfType<AddOperation>();
		error.Error.Should().Be(0.00345M);
	}
	
	[Fact]
	public void PlusOperator_withIncompatibleMoneys_throwsException()
	{
		// Arrange
		var context = new MonetaContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.00, UsDollar);
		
		// Act and Assert
		money1.Invoking(x => x + money2)
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Currencies 'EUR' and 'USD' are not compatible.");
	}

	[Fact]
	public void PlusOperator_api_coverage_test()
	{
		var context = new MonetaContext();
		var sut = context.CreateMoney(1M);
		var expected = context.CreateMoney(2M);
		
		(sut + 1).Should().Be(expected);
		(sut + 1M).Should().Be(expected);
		(sut + 1L).Should().Be(expected);
		(sut + 1U).Should().Be(expected);
		(sut + 1UL).Should().Be(expected);
		(sut + 1F).Should().Be(expected);
		(sut + 1D).Should().Be(expected);
	}
}