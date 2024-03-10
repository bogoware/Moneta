using Bogoware.Moneta.CurrencyProviders;
using Bogoware.Moneta.Exceptions;
using static System.MidpointRounding;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneySubtractTests : MoneyBaseTests
{
	[Fact]
	public void Subtracting_cant_create_value()
	{
		// Arrange
		var context = MonetaContext.Create(options =>
		{
			options.DefaultCurrency = Euro;
			options.RoundingMode = ToPositiveInfinity;
		});
		var sut = context.CreateMoney(0.99M);
		const decimal other = -0.001M;

		// Act
		var result = sut.Subtract(other, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1M);
		const decimal expectedError = -0.009M;

		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}

	[Fact]
	public void Subtracting_cant_destroy_value()
	{
		// Arrange
		var context = MonetaContext.Create(options =>
		{
			options.DefaultCurrency = Euro;
			options.RoundingMode = ToNegativeInfinity;
		});
		var sut = context.CreateMoney(1.00M);
		const decimal other = -0.001M;

		// Act
		var result = sut.Subtract(other, out var error);

		// Arrange
		var expectedResult = context.CreateMoney(1M);
		const decimal expectedError = 0.001M;

		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
	}

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
	public void Subtracting_withError_isSafe()
	{
		// Arrange
		var context = MonetaContext.Create(options => options.DefaultCurrency = Euro);
		var sut = context.CreateMoney(1.00M);
		const double other = 0.12345;

		// Act
		var result = sut.Subtract(other, out var error);

		// Assert
		var expectedResult = context.CreateMoney(0.88M);
		const decimal expectedError = -0.00345M;

		result.Should().Be(expectedResult);
		error.Should().Be(expectedError);
		context.HasRoundingErrors.Should().BeFalse();
	}

	[Fact]
	public void Subtracting_withoutError_isUnsafe()
	{
		// Arrange
		var context = MonetaContext.Create(options => options.DefaultCurrency = Euro);
		var sut = context.CreateMoney(1.00M);
		const double amount = 0.12345;

		// Act
		var result = sut.Subtract(amount);

		// Assert
		var expectedResult = context.CreateMoney(0.88M);
		const decimal expectedError = -0.00345M;

		result.Should().Be(expectedResult);
		context.HasRoundingErrors.Should().BeTrue();
		context.RoundingErrors.Should().HaveCount(1);
		var error = context.RoundingErrors[0];
		error.Should().BeOfType<SubtractOperationError>();
		error.Error.Should().Be(expectedError);
	}

	[Fact]
	public void MinusOperator_withIncompatibleMoneys_throwsException()
	{
		// Arrange
		var context = new MonetaContext();
		var money1 = context.CreateMoney(1.00, Euro);
		var money2 = context.CreateMoney(1.00, UsDollar);

		// Act and Assert
		money1.Invoking(x => x - money2)
			.Should().Throw<CurrencyIncompatibleException>()
			.WithMessage("Currencies 'EUR' and 'USD' are not compatible.");
	}

	[Fact]
	public void MinusOperator_api_coverage_test()
	{
		var context = new MonetaContext();
		var sut = context.CreateMoney(1);
		var expected = context.CreateMoney(0);

		(sut - 1).Should().Be(expected);
		(sut - 1M).Should().Be(expected);
		(sut - 1L).Should().Be(expected);
		(sut - 1U).Should().Be(expected);
		(sut - 1UL).Should().Be(expected);
		(sut - 1F).Should().Be(expected);
		(sut - 1D).Should().Be(expected);
	}

	[Fact]
	public void MinusOperator_ByTwoEqualAmounts_GetZero()
	{
		using var context = MonetaContext.Create(options =>
		{
			options.CurrencyProvider = new IsoCurrencyProvider();
			options.DefaultCurrency = options.CurrencyProvider.GetCurrency("EUR");
		});

		var lhs = context.CreateMoney(1000);
		var rhs = context.CreateMoney(1000);
		var expected = context.CreateMoney(0);

		var result = lhs - rhs;
		
		result.Should().Be(expected);
	}
}