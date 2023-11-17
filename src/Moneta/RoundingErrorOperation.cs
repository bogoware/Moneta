namespace Bogoware.Moneta;

/// <summary>
/// Represents an error rounding operation.
/// </summary>
/// <param name="Error">The residual part</param>
/// <param name="Currency">The currency</param>
public abstract record RoundingErrorOperation(decimal Error, ICurrency Currency);

public sealed record CreateOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
// ReSharper disable once NotAccessedPositionalProperty.Global
public sealed record SplitOperation(Money Unallocated) : RoundingErrorOperation(Unallocated.Amount, Unallocated.Currency);
public sealed record DivideOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
public sealed record MultiplyOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
public sealed record AddOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
public sealed record SubtractOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
public sealed record MapOperation(decimal Error, ICurrency Currency) : RoundingErrorOperation(Error, Currency);
public sealed record RoundOffOperation(Money Unallocated) : RoundingErrorOperation(Unallocated.Amount, Unallocated.Currency);
