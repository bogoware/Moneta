// ReSharper disable NotAccessedPositionalProperty.Global
namespace Bogoware.Moneta;

/// <summary>
/// Represents an error rounding operation.
/// </summary>
/// <param name="Error">The residual part</param>
/// <param name="Currency">The currency</param>
public abstract record RoundingOperationError(decimal Error, ICurrency Currency);

public sealed record CreateOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record SplitOperationError(Money Unallocated) : RoundingOperationError(Unallocated.Amount, Unallocated.Currency);
public sealed record ApplyOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record DivideOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record MultiplyOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record AddOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record SubtractOperationError(decimal Error, ICurrency Currency) : RoundingOperationError(Error, Currency);
public sealed record RoundOffOperationError(Money Unallocated) : RoundingOperationError(Unallocated.Amount, Unallocated.Currency);
