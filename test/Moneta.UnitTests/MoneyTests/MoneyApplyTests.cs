using Bogoware.Moneta.Abstractions;
using Bogoware.Moneta.CommonCurrencies;

namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyApplyTests
{
	public static readonly Func<decimal, decimal> SafeDecimalTransformation = m => m * 2 + 0.23m;

	public static readonly Func<decimal, decimal> UnsafeDecimalTransformation = m => (decimal)Math.Sin((double)m);

	public static readonly Func<Money, decimal> SafeMoneyTransformation = m => SafeDecimalTransformation(m.Amount);

	public static readonly Func<Money, decimal> UnsafeMoneyTransformation = m => UnsafeDecimalTransformation(m.Amount);

	[Fact]
	public void Apply_withMoney_worksWithoutResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);
		
		// Act
		var result = sut.Apply(SafeMoneyTransformation, out var residue);
		
		// Assert
		result.Amount.Should().Be(20.23M);
		residue.Should().Be(0M);
	}

	[Fact]
	public void Apply_withDecimal_worksWithoutResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M);
		
		// Act
		var result = sut.Apply(SafeDecimalTransformation, out var residue);
		
		// Assert
		result.Amount.Should().Be(20.23M);
		residue.Should().Be(0M);
	}

	[Fact]
	public void Apply_withMoney_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(1M);
		
		// Act
		var result = sut.Apply(UnsafeMoneyTransformation, out var residue);
		
		// Assert
		result.Amount.Should().Be(0.84M);
		residue.Should().NotBe(0M);
	}

	[Fact]
	public void Apply_withDecimal_worksWithResidue()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(1M);
		
		// Act
		var result = sut.Apply(UnsafeDecimalTransformation, out var residue);
		
		// Assert
		result.Amount.Should().Be(0.84M);
		residue.Should().NotBe(0M);
	}
	
	[Fact]
	public void Apply_withDecimal_interceptRoundingErrors()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(1M);
		
		// Act
		var result = sut.Apply(UnsafeDecimalTransformation);
		
		// Assert
		result.Amount.Should().Be(0.84M);
		moneyContext.HasRoundingErrors.Should().BeTrue();
		moneyContext.RoundingErrors.Should().HaveCount(1)
			.And.ContainItemsAssignableTo<ApplyOperationError>();
		moneyContext.ClearRoundingErrors();
	}

	[Fact]
	public void Apply_transformsEurosToDollars()
	{
		// Arrange
		var moneyContext = new MonetaContext();
		var sut = moneyContext.CreateMoney(10M, Euro.Instance);
		
		// Act
		var result = sut.Apply(m => m.Context.CreateMoney(m.Amount, Dollar.Instance));
		
		// Assert
		result.Currency.Should().Be(Dollar.Instance);
		result.Amount.Should().Be(10M);
	}
}