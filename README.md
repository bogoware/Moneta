# Moneta

Moneta is a library designed to support monetary calculations in a secure manner.

### Summary

* [Monetary Context](#monetary-context)
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

### Monetary Context

A `MonetaryContext` sets the boundaries for safe and coherent monetary operations, ensuring that no rounding errors go unnoticed.

A `MonetaryContext` defines the following:
* The default `ICurrency` for creating new `Money` instances. For example, if you exclusively deal with EUR, you can set EUR as the default currency, making the creation of new `Money` instances more straightforward.
* The `ICurrencyProvider` used to resolve currencies by code, enabling you to resolve currencies from a database or a web service.
* The default `RoundingMode` for monetary operations.
* The decimal precision used to detect rounding errors (see `RoundingErrorDecimals`). By default, all internal operations are rounded to 8 decimal places, but you can change this value up to 28 decimal places. For instance, this is useful for operations involving cryptocurrencies, which often have many decimal places.
* A log of operations that have resulted in rounding errors (according to the `RoundingErrorDecimals` value).

### Money

`Money` is Moneta's data type for representing monetary values. It consists of a `decimal` value associated with a `Currency`.

The supported operations are:
* `Split`: will split a `Money` into a list of `Money` instances according to the specified `RoundingMode` and number of parts or weights.
* `Apply`: will apply a function to the `Money` value and return a new `Money` instance with the same `Currency` and the rounding error of the operation.
* `Map`: will apply a function to the `Money` value and return a new `Money` instance with the same `Currency`.
* `Add`, `Subtract`, `Multiply`, `Divide`: will perform the corresponding operation between two `Money` instances or a `Money` instance and a number, returning a new `Money` instance with the same `Currency`.
* `Negate`: will negate the `Money` value and return a new `Money` instance with the same `Currency`.
* `CompareTo`: will compare the `Money` value with another `Money` instance or a number. If the `Money` instances have different `Currencies`, and the `MonetaryContext` has an `IExchangeRateProvider`, the `Money` instances will be converted to the same `Currency` before the comparison, otherwise an exception will be thrown.

And of course you can also use basic operators such as `+`, `-`, `*`, `/`, `==`, `!=`, `>`, `>=`, `<`, `<=`. All the binary operators between a `Money` instance and a number are unsafe operations (see [Safe and Unsafe Operations](#safe-and-unsafe-operations)).

### Currency

In Moneta, a Currency is any instance of `Currency<TSelf>`. There are no strict constraints on what qualifies as a valid currency; you are free to introduce your own currency as long as it fits your domain. The only requirement is that currencies with the same `Code` are treated as equivalent.

#### Currency Decimal Places

An important characteristic of a currency is the number of decimal places it supports. When you perform a monetary operation, the result is rounded based on the currency's decimal precision, potentially resulting in rounding errors.

### Rounding Error Detection

Moneta aims to facilitate secure monetary calculations through a fluent algebraic API. The design goal is to provide an API that allows you to conduct monetary operations within a secure context, detecting and reporting any potential rounding errors. This way, you can manage them according to your domain requirements.

A *rounding error* occurs when an operation cannot be performed without losing precision. These errors are influenced not only by the *values* involved in the operation but also by the `RoundingMode` and the `MonetaryContext.RoundingErrorDecimals`.

There are two main causes of rounding errors:
* Split Operations
* Floating Point Operations

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

### Creating a Monetary Context

```csharp

```