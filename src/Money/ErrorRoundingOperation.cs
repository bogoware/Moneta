namespace Bogoware.Money;

/// <summary>
/// Represents an error rounding operation.
/// </summary>
/// <param name="Residue">The residual part</param>
/// <param name="Currency">The currency</param>
public abstract record ErrorRoundingOperation(decimal Residue, Currency Currency);

public sealed record ConvertFromDoubleOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record SplitOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record DivideOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record MultiplyOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record AddOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
public sealed record SubtractOperation(decimal Residue, Currency Currency) : ErrorRoundingOperation(Residue, Currency);
