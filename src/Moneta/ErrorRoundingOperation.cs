namespace Bogoware.Moneta;

/// <summary>
/// Represents an error rounding operation.
/// </summary>
/// <param name="Residue">The residual part</param>
/// <param name="Currency">The currency</param>
public abstract record ErrorRoundingOperation(decimal Residue, ICurrency Currency);

public sealed record CreateOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record SplitOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record DivideOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record MultiplyOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record AddOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record SubtractOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);

public sealed record MapOperation(decimal Residue, ICurrency Currency) : ErrorRoundingOperation(Residue, Currency);
