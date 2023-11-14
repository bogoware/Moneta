
// ReSharper disable RedundantArgumentDefaultValue

using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyAddTests : MoneyBaseTests
{
	[Fact]
	public void Adding_incompatibleMoney_throwsException()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00M, Euro);
		var other = context.CreateMoney(1.00M, UsDollar);
		
		// Act and Assert
		sut.Invoking(x => x.Add(other))
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Money 'EUR 1.00' and 'USD 1.00' are not compatible.");
	}
	
	[Fact]
	public void Adding_compatibleMoneys_withError_isSafe()
	{
		// Arrange
		var context = new MonetaryContext(defaultCurrency: Euro);
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
	public void Adding_compatibleMoneys_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaryContext(defaultCurrency: Euro);
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
	public void Adding_compatibleMoneys_withSameDecimals_withError_isSafe()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.23, UndefinedCurrencyWith2Digits);

		// Act
		var result = sut.Add(other, out var error);

		// Assert
		result.Amount.Should().Be(2.23M);
		result.Currency.Should().Be(Euro);
		error.Should().Be(0.00M);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Adding_compatibleMoneys_withSameDecimals_withoutError_isSafe()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.23, UndefinedCurrencyWith2Digits);

		// Act
		var result = sut.Add(other);

		// Assert
		result.Amount.Should().Be(2.23M);
		result.Currency.Should().Be(Euro);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Adding_compatibleMoneys_withDifferentDecimals_withError_isSafe()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.2345, UndefinedCurrencyWith4Digits);

		// Act
		var result = sut.Add(other, out var error);

		// Assert
		result.Amount.Should().Be(2.23M);
		result.Currency.Should().Be(Euro);
		context.HasRoundingErrors.Should().BeFalse();
		error.Should().Be(0.0045M);
	}

	[Fact]
	public void Adding_compatibleMoneys_withDifferentDecimals_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.2345, UndefinedCurrencyWith4Digits);

		// Act
		var result = sut.Add(other);

		// Assert
		result.Amount.Should().Be(2.23M);
		result.Currency.Should().Be(Euro);
		context.HasRoundingErrors.Should().BeTrue();

		context.RoundingErrors.Should().HaveCount(1);
		var errorRoundingOperation = context.RoundingErrors[0];
		errorRoundingOperation.Should().BeOfType<AddOperation>();
		errorRoundingOperation.As<AddOperation>().Error.Should().Be(0.0045M);
	}

	[Fact]
	public void Adding_compatibleMoneys_withDifferentDecimals_withError_isSafe_andReturnsTheMostPreciseCurrency()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, UndefinedCurrencyWith2Digits);
		var other = context.CreateMoney(1.2345, UndefinedCurrencyWith4Digits);
		
		// Act
		var result = sut.Add(other, out var error);
		
		// Assert
		result.Amount.Should().Be(2.2345M);
		result.Currency.Should().Be(UndefinedCurrencyWith4Digits);
		error.Should().Be(0);
		context.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void Adding_compatibleMoneys_withDifferentDecimals_withoutError_isSafe_andReturnsTheMostPreciseCurrency()
	{
		// Arrange
		var context = new MonetaryContext();
		var sut = context.CreateMoney(1.00, UndefinedCurrencyWith2Digits);
		var other = context.CreateMoney(1.2345, UndefinedCurrencyWith4Digits);
		
		// Act
		var result = sut.Add(other);
		
		// Assert
		result.Amount.Should().Be(2.2345M);
		result.Currency.Should().Be(UndefinedCurrencyWith4Digits);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Adding_floatingPointNumber_withError_isSafe()
	{
		// Arrange
		var context = new MonetaryContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1);
		var amount = 0.12345;
		
		// Act
		var result = sut.Add(amount, out var error);
		
		// Assert
		var expected = context.CreateMoney(1.12M);
		result.Should().Be(expected);
		error.Should().Be(0.00345M);
		context.HasRoundingErrors.Should().BeFalse();
	}
	
	[Fact]
	public void Adding_floatingPointNumber_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaryContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1);
		var amount = 0.12345;
		
		// Act
		var result = sut.Add(amount);
		
		// Assert
		var expected = context.CreateMoney(1.12M);
		result.Should().Be(expected);
		context.HasRoundingErrors.Should().BeTrue();
		var error = context.RoundingErrors.First();
		error.Should().BeOfType<AddOperation>();
		error.Error.Should().Be(0.00345M);
	}

	[Fact]
	public void PlusOperator_withIncompatibleMoneys_throwsException()
	{
		// Arrange
		var context = new MonetaryContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.00, UsDollar);
		
		// Act and Assert
		money1.Invoking(x => x + money2)
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Money 'EUR 1.00' and 'USD 1.00' are not compatible.");
	}

	[Fact]
	public void PlusOperator_withCompatibleMoneys_isUnsafe()
	{
		// Arrange
		var context = new MonetaryContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.2345, UndefinedCurrencyWith4Digits);
		
		// Act
		var result = money1 + money2;
		
		// Assert
		result.Amount.Should().Be(2.23M);
		context.HasRoundingErrors.Should().BeTrue();
		var error = context.RoundingErrors.First();
		error.Should().BeOfType<AddOperation>();
		error.Error.Should().Be(0.0045M);
	}
}