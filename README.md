# Moneta

Moneta is a library designed to support monetary calculations in a secure manner.

### Summary

* [Moneta Context](#moneta-context)
* [The Principle of Monetary Value Conservation](#the-principle-of-monetary-value-conservation)
  * [Rounding Error Detection](#rounding-error-detection)
* [Money](#money)
* [Supported Operations](#supported-operations)
  * [Split](#split)
  * [Apply](#apply)
  * [Map](#map): suitable for functional style programming
  * [Bind](#bind): suitable for functional style programming
  * [Add](#add)
  * [Subtract](#subtract)
  * [Multiply](#multiply)
  * [Divide](#divide)
  * [Negate](#negate)
  * [CompareTo](#compareto)
  * [Binary Operators (+, -, *, /, <, <=, …)](#operators)
* [Currency](#currency)
* [Rounding Error Detection](#rounding-error-detection)
* [Safe and Unsafe Operations](#safe-and-unsafe-operations)
* [Currency System](#currency-system)
* [Currency Providers](#currency-providers)
* [Exchange Rate Conversion](#exchange-rate-conversion) TODO
* [Exchange Rate Providers](#exchange-rate-providers) TODO
* [Samples](#samples) TODO
* [Dependency Injection](#dependency-injection) TODO

## Concepts and Key Features

### Moneta Context

A `MonetaContext` sets the boundaries for safe and coherent monetary operations, ensuring that no rounding errors go unnoticed.

A `MonetaContext` defines the following:
* The default `ICurrency` for creating new `Money` instances. For example, if you exclusively deal with EUR, you can set EUR as the default currency, making the creation of new `Money` instances more straightforward.
* The `ICurrencyProvider` used to resolve currencies by code, enabling you to resolve currencies from a database or a web service.
* The default `RoundingMode` for monetary operations.
* The decimal precision used to detect rounding errors (see `RoundingErrorDecimals`). By default, all internal operations are rounded to 8 decimal places, but you can change this value up to 28 decimal places. For instance, this is useful for operations involving cryptocurrencies, which often have many decimal places.
* A log of operations that have resulted in rounding errors (according to the `RoundingErrorDecimals` value).

### The Principle of Monetary Value Conservation

In Moneta holds the «Monetary Value Conservation Principle»: no monetary value is created or lost during the lifetime of a `MonetaContext`.

Moneta will provide means to keep track of any monetary value created or lost during the lifetime of a `MonetaContext`
and make it available to the user who can decide how to handle it.

There are basically two main causes of monetary value creation or loss:
* Split Operations: which can produce an unallocated part, both positive or negative depending by the `RoundingMode` used.
* Floating Point Operations with floating point numbers more precise than the `Currency` of the `Money` involved in the operation.

#### Rounding Error Detection

A *rounding error* occurs when an operation cannot be performed without losing precision.
These errors are influenced not only by the *values* involved in the operation but also by the `RoundingMode` and
the `MonetaContext.RoundingErrorDecimals` that is used by the `MonetaContext` to detect rounding errors.

> [!IMPORTANT]
> `MonetaContext` will prevent you to create any `Money` that uses a `Currency` with a number of `DecimalPlaces` greather than the `RoundingErrorDecimals` value.

> [!NOTE]
> Using a `RoundingErrorDecimals` equals to the `Currency` with the highest number of `DecimalPlaces` involved in your tasks will prevent any rounding error to go noticed.

Every result of an operation involving a floating point operand is rounded to the `MonetaContext.RoundingErrorDecimals`
before the monetary value is computed.
Thi value is then rounded accordingly to the decimals required by the `Currency` of the `Money` involved.
The difference between the two values is the rounding error.
 
Every safe operation will return a `decimal` value or, in the case of the [Split](#split-operation), a `Money` value representing the amount of value created or destroyed.
Unsafe operations, instead, will keep track of the operation and the error occurred.

Schematically, let's assume that:
* `•` is the operation performed
* `M` is the `Money` involved in the operation
* `V` is the value (`decimal`, `double` or `float`) involved in the operation
* `R` is the `Money` returned by the operation
* `E` is the `error` returned by the operation

then the following equation holds:

```
M • V = R + E
```

More precisely, the following equation holds:
```math
\left( \sum_{k=1}^n a_k b_k \right)^2 \leq \left( \sum_{k=1}^n a_k^2 \right) \left( \sum_{k=1}^n b_k^2 \right)
```


> [!NOTE]
> `error` is positive in case of monetary value lost and negative in case of monetary value created.

>  [!IMPORTANT]
> `error` is always a `decimal` value with `MonetaContext.RoundingErrorDecimals` decimals at most.

For example, if you perform the operation `1.00 EUR + 0.1234` with rounding mode `ToZero` or `ToEven` you will get `1.12 EUR` with a rounding error of `0.0034`,
which is the amount of monetary value lost during the operation.
But if you perform the same operation with rounding mode `ToPositiveInfinity` you will get `1.13 EUR` with a rounding error of `-0.0066`,
which is the amount of monetary value created during the operation.

It's up to you to decide which treatment to apply to the rounding errors depending by your domain requirements.

### Moneta API Design

Moneta main design goal is to support safe monetary calculations through a fluent algebraic API.

For every supported operation, Moneta provides two set of overloads:
* *safe* overloads that returns the rounding error or, in the case of the `Spilt` operations, the unallocated part
* *unsafe* overloads that doesn't return the created or lost monetary value and relies on the `MonetaContext` to keep track of it.

The `error` or `unallocated` part returned by the safe overloads represents a quantity of the monetary value that has been created or lost during the operation.

### Money

`Money` is Moneta's type for representing monetary values. It consists of a `decimal` value associated with a `Currency`.

The supported operations are:
* `Split`: will split a `Money` into a list of `Money` instances according to the specified `RoundingMode` and number of parts or weights.
* `Apply`: will apply a function to the `Money` value and return a new `Money` instance with the same `Currency` and the rounding error of the operation.
* `Map`: will apply a function to the `Money` value and return a new `Money` instance with the same `Currency`.
* `Add`, `Subtract`, `Multiply`, `Divide`: will perform the corresponding operation between two `Money` instances or a `Money` instance and a number, returning a new `Money` instance with the same `Currency`.
* `Negate`: will negate the `Money` value and return a new `Money` instance with the same `Currency`.
* `CompareTo`: will compare the `Money` value with another `Money` instance or a number. If the `Money` instances have different `Currencies`, and the `MonetaContext` has an `IExchangeRateProvider`, the `Money` instances will be converted to the same `Currency` before the comparison, otherwise an exception will be thrown.

And of course you can also use basic operators such as `+`, `-`, `*`, `/`, `==`, `!=`, `>`, `>=`, `<`, `<=`. All the binary operators between a `Money` instance and a number are unsafe operations (see [Safe and Unsafe Operations](#safe-and-unsafe-operations)).

### Currency

In Moneta, a Currency is any instance of `Currency<TSelf>`. There are no strict constraints on what qualifies as a valid currency; you are free to introduce your own currency as long as it fits your domain. The only requirement is that currencies with the same `Code` are treated as equivalent.

#### Currency Decimal Places

An important characteristic of a currency is the number of decimal places it supports. When you perform a monetary operation, the result is rounded based on the currency's decimal precision, potentially resulting in rounding errors.

##### Rounding Errors in Split Operations

Rounding errors in split operations occur when you attempt to divide a `Money` into parts that cannot be divided fairly according to the `RoundingMode` used. For example, if you try to divide 1.00 EUR into 3 parts, the result will be 0.33 EUR with a rounding error of 0.01 EUR. Similar situations arise when you perform a *weighted* split operation using floating point weights.

##### Rounding Errors in Algebraic Operations

Rounding errors in algebraic operations occur when you perform an operation between a `Money` and a floating point number with a decimal part that exceeds the decimal places supported by the target `Currency`. For instance, if you try to add 1.12345678 EUR to 1.12 EUR, the result will be 2.24 EUR with a rounding error of 0.00345678 EUR.

### Safe and Unsafe Operations

Moneta's API offers two types of operations: *safe* and *unsafe* operations.

*Safe operations* are operations that return the `error` of the operation to the caller and relieve the context from tracking rounding errors. The caller is responsible for handling the error in line with domain requirements.

In contrast, *unsafe operations* do not return the `error` of the operation, and the context is responsible for keeping track of rounding errors. The caller must check the context for any rounding errors and address them accordingly.

In particular:
* All binary operations between a `Money` value and a floating point number are unsafe operations.
* All `Money.Map` operations are unsafe operations.

### Supported Operations

In the following table there'is a recap of all the supported operations and their safety.
An operation is considered safe if it can generate a rounding error and return it to the caller
or it cannot generate a rounding error in any case.

For example, the `Split` operation provides different overload methods, both safe and unsafe. 

Operation | Safe | Unsafe | Notes
--- |----|-----| ---
`MonetaContext.Create` | Yes | Yes | Allocate a new `Money`.
`Split` | Yes | Yes | Split the value of a `Money` into a list of `Money` instances. There are two overloads: one that takes the number of parts and one that takes a list of weights. Split methods, instead of the rounding error, will return the more significative unallocated part as `Money`.
`Apply` | Yes | Yes | Apply a function to the `Money`
`Map` | No | Yes | Similaer to `Aplply` but it doesn't return the rounding error. Suitable for functional style programming.
`Add` | Yes | Yes | Adds a numeric value or a compatible `Money`
`Subtract` | Yes | Yes | Subtracts a numeric value or a compatible `Money`
`Multiply` | Yes | Yes | Multiplies for a numeric value
`Divide`| Yes | Yes | Divides by a numeric value
`Negate` | Yes | No  | Negates the `Money` amount
+, -, *, / with floating point numbers| No | Yes | The arithmetic operators. Binary operators between a `Money` value and a floating point number are unsafe operations.
+, - with `Money` values with the same `Currency` or integral numbers or between any kind of `UndefinedCurrency` | Yes | No  | Binary operators between `Money` values with the same `Currency` or integral numbers are safe operations
+, - with `Money` values with `UndefinedCurrency` with different `DecimalPlaces` | No | Yes | Binary operators between `Money` values with `UndefinedCurrency` a number of `DecimalPlaces` greather than the value with a defined currency are unsafe.


### Currency System

#### Undefined Currency

The Undefined Currency is a special currency identified by the ISO 4217 code `XXX`. It represents a `Money` without a currency. You can create multiple variants of `UndefinedCurrency` with different `DecimalPlaces`.

#### Currency Compatibility and Binary Operations

Two `Currencies` are considered compatible if they share the same `Code` or if one of them is `UndefinedCurrency`.

Binary operations between two `Money` instances are allowed only if their `Currencies` are compatible.

### Currency Providers

Moneta offers numerous extension points for customizing the library's behavior. One of the most important is the `ICurrencyProvider`, which enables you to seamlessly integrate your currency system. For example, you can implement an `ICurrencyProvider` to resolve currencies from a database or a web service.

Moneta provides the following providers:
* `NullCurrencyProvider`, which doesn't resolve any currency.
* `IsoCurrencyProvider`, which resolves currencies from the ISO 4217 standard. This provider can be customized to resolve a subset of the standard.
* `ChainOfCurrencyProvider`, which resolves currencies from a list of other providers, suitable for caching and fallback scenarios.

### Exchange Rate Conversion

TBD (To Be Determined)

### Exchange Rate Providers

TBD (To Be Determined)

## Samples

These samples are available in the [Moneta.Samples](./samples/MonetaHelloWorld) project.

### Sample 1: no rounding errors occurred

```csharp
using Bogoware.Moneta;
using Bogoware.Moneta.CurrencyProviders;

using (var context = new MonetaContext("EUR", new IsoCurrencyProvider()))
{
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;
	
	Console.WriteLine($"The final amount is {money}");
} // OK!
```
### Sample 2: handled rounding errors

```csharp
using (var context = new MonetaContext("USD", new IsoCurrencyProvider()))
{
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;
	money += 1.2321; // Unhandled Rounding error
	
	if (context.HasRoundingErrors)
	{
		Console.WriteLine(" > Rounding errors detected");
		foreach (var error in context.RoundingErrors)
		{
			// TODO: Handle rounding errors
			Console.WriteLine($"   Error: {error}");
		}
		context.ClearRoundingErrors();
	}
	
	Console.WriteLine($"The final amount is {money}");
} // OK!
```
### Sample 3: unhanded rounding errors

```csharp
using (var context = new MonetaContext("USD", new IsoCurrencyProvider()))
{
	var money = context.CreateMoney(1.00M);

	money += 11;
	money /= 2;
	money += 1.2321; // Unhandled Rounding error
	
	Console.WriteLine($"The final amount is {money}");
} // KO! Exception thrown
```

###  Sample 4: weighted split with unallocated money and rounding error

```csharp
using (var context = new MonetaContext("EUR", new IsoCurrencyProvider()))
{
    var money = context.CreateMoney(11.11);
    var weights = Enumerable.Repeat(0.333333, 3);

	var split = money.Split(weights, out var unallocated);

	Console.WriteLine($"The original amount is {money}");
	Console.WriteLine($"The allocated amounts are: {string.Join(", ", split)}");
	Console.WriteLine($"The unallocated amount is {unallocated}");
} // OK!
```

will produce the following output:

```
The original amount is EUR 11.11
The allocated amounts are: EUR 3.70, EUR 3.70, EUR 3.70
The unallocated amount is EUR 0.01

```
