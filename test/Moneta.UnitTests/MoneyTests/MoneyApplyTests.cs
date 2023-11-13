namespace Bogoware.Moneta.UnitTests.MoneyTests;

public class MoneyApplyTests
{
	public static readonly Func<decimal, decimal> SafeDecimalTransformation = m => m * 2 + 0.23m;
	public static readonly Func<decimal, decimal> UnsafeDecimalTransformation = m => (decimal)Math.Sin((double)m);
	
	public static readonly Func<Moneta.Money, decimal> SafeMoneyTransformation = m => SafeDecimalTransformation(m.Amount);
	public static readonly Func<Moneta.Money, decimal> UnsafeMoneyTransformation = m => UnsafeDecimalTransformation(m.Amount);
	
	[Fact]
	public void Apply_withMoney_worksWithoutResidue()
	{
		// Arrange
		var moneyContext = new MonetaryContext();
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
		var moneyContext = new MonetaryContext();
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
		var moneyContext = new MonetaryContext();
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
		var moneyContext = new MonetaryContext();
		var sut = moneyContext.CreateMoney(1M);
		
		// Act
		var result = sut.Apply(UnsafeDecimalTransformation, out var residue);
		
		// Assert
		result.Amount.Should().Be(0.84M);
		residue.Should().NotBe(0M);
	}
}