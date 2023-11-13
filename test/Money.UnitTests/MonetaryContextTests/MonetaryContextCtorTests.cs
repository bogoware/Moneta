namespace Bogoware.Money.UnitTests.MonetaryContextTests;

public class MonetaryContextCtorTests
{

	[Fact]
	public void MonetaryContext_voidCtor()
	{
		// Act
		var sut = new MonetaryContext();
		
		// Assert
		sut.DefaultCurrency.Should().Be(Currency.Undefined);
		sut.RoundingMode.Should().Be(MidpointRounding.ToEven);
		sut.OperationDecimalPlaces.Should().Be(8);
		sut.HasRoundingErrors.Should().BeFalse();
		sut.ErrorRoundingOperations.Should().BeEmpty();
	}

	private class FakeCurrency : Currency<FakeCurrency>
	{
		public static ICurrency Instance { get; } = new FakeCurrency();
		private FakeCurrency()
			: base("Fake", "Fake", "Fake", 2)
		{
		}
	}
	
	[Fact]
	public void MonetaryContext_customCtor()
	{
		// Act
		var sut = new MonetaryContext(FakeCurrency.Instance, MidpointRounding.AwayFromZero, 4);
		
		// Assert
		sut.DefaultCurrency.Should().Be(FakeCurrency.Instance);
		sut.RoundingMode.Should().Be(MidpointRounding.AwayFromZero);
		sut.OperationDecimalPlaces.Should().Be(4);
		sut.HasRoundingErrors.Should().BeFalse();
		sut.ErrorRoundingOperations.Should().BeEmpty();
	}
}