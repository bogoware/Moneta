using Bogoware.Moneta.CurrencyProviders;

namespace Bogoware.Moneta.UnitTests.IsoCurrencyProviderTests;

public class IsoCurrencyProviderTests
{
	[Theory]
	[InlineData("AED")]
	[InlineData("EUR")]
	[InlineData("USD")]
	public void UnfilteredCodes_should_resolve_valid_codes(string code)
	{
		// Arrange
		var sut = new IsoCurrencyProvider();
		
		// Act
		var result = sut.TryGetCurrency(code, out var currency);
		
		// Assert
		result.Should().BeTrue();
		currency.Should().NotBeNull();
	}
	
	[Fact]
	public void UnfilteredCodes_should_not_resolve_invalid_codes()
	{
		// Arrange
		var sut = new IsoCurrencyProvider();
		
		// Act
		var result = sut.TryGetCurrency("XXX", out var currency);
		
		// Assert
		result.Should().BeFalse();
		currency.Should().BeNull();
	}
	
	[Fact]
	public void FilteredCodes_should_resolve_valid_codes()
	{
		// Arrange
		var sut = new IsoCurrencyProvider(new[] {"EUR", "USD"});
		
		// Act
		var result = sut.TryGetCurrency("EUR", out var currency);
		
		// Assert
		result.Should().BeTrue();
		currency.Should().NotBeNull();
	}
	
	[Fact]
	public void FilteredCodes_should_not_resolve_invalid_codes()
	{
		// Arrange
		var sut = new IsoCurrencyProvider(new[] {"EUR", "USD"});
		
		// Act
		var result = sut.TryGetCurrency("XXX", out var currency);
		
		// Assert
		result.Should().BeFalse();
		currency.Should().BeNull();
	}
}