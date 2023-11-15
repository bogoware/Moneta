using Bogoware.Moneta.Exceptions;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneySubtractTests : MoneyBaseTests
{
	[Fact]
	public void Subtracting_incompatibleMoney_throwsException()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00M, Euro);
		var other = context.CreateMoney(1.00M, UsDollar);
		
		// Act and Assert
		sut.Invoking(x => x.Subtract(other))
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Currencies 'EUR' and 'USD' are not compatible.");
	}
	
	[Fact]
	public void Subtracting_compatibleMoneys_withError_isSafe()
	{
		// Arrange
		var context = new MonetaContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1.00M);
		const double amount = 0.12345;

		// Act
		var result = sut.Subtract(amount, out var error);

		// Assert
		result.Amount.Should().Be(0.88M);
		error.Should().Be(0.00345M);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Subtracting_compatibleMoneys_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaContext(defaultCurrency: Euro);
		var sut = context.CreateMoney(1.00M);
		var amount = 0.12345;

		// Act
		var result = sut.Subtract(amount);

		// Assert
		result.Amount.Should().Be(0.88M);
		context.HasRoundingErrors.Should().BeTrue();
		context.RoundingErrors.Should().HaveCount(1);
		var error = context.RoundingErrors.First();
		error.Should().BeOfType<SubtractOperation>();
		error.Error.Should().Be(0.00345M);
	}

	[Fact]
	public void Subtracting_compatibleMoneys_withSameDecimals_withError_isSafe()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.23, UndefinedCurrency.Instance);

		// Act
		var result = sut.Subtract(other, out var error);

		// Assert
		result.Amount.Should().Be(-0.23M);
		result.Currency.Should().Be(Euro);
		error.Should().Be(0.00M);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Subtract_compatibleMoneys_withSameDecimals_withoutError_isSafe()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.23, UndefinedCurrency.Instance);

		// Act
		var result = sut.Subtract(other);

		// Assert
		result.Amount.Should().Be(-0.23M);
		result.Currency.Should().Be(Euro);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Subtract_compatibleMoneys_withDifferentDecimals_withError_isSafe()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.2345, UndefinedCurrency.Instance);

		// Act
		var result = sut.Subtract(other, out var error);

		// Assert
		result.Amount.Should().Be(-0.23M);
		result.Currency.Should().Be(Euro);
		context.HasRoundingErrors.Should().BeFalse();
		error.Should().Be(0.0045M);
	}

	[Fact]
	public void Adding_compatibleMoneys_withDifferentDecimals_withoutError_isUnsafe()
	{
		// Arrange
		var context = new MonetaContext();
		var sut = context.CreateMoney(1.00, Euro);
		var other = context.CreateMoney(1.2345, UndefinedCurrency.Instance);

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
	public void Adding_floatingPointNumber_withError_isSafe()
	{
		// Arrange
		var context = new MonetaContext(defaultCurrency: Euro);
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
		var context = new MonetaContext(defaultCurrency: Euro);
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
		var context = new MonetaContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.00, UsDollar);
		
		// Act and Assert
		money1.Invoking(x => x + money2)
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Currencies 'EUR' and 'USD' are not compatible.");
	}

	[Fact]
	public void PlusOperator_withCompatibleMoneys_isUnsafe()
	{
		// Arrange
		var context = new MonetaContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.2345, UndefinedCurrency.Instance);
		
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