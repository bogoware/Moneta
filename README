# Moneta

Moneta is a library to support monetary calculations in a safe manner.

## Concepts and Key Features

### Monetary Context

A `MonetaryContext` is a boundary of safety and coherence within you can perform monetary operations assured
that no rounding error will escape from your scrutiny.

A `MonetaryContext` defines:
* The default `ICurrency` to use to create new `Money`: for example if you only work with EUR, 
  you can set EUR as default currency simplifying the creation of new `Money` instances
* The `ICurrencyProvider` to use to resolve currencies by code: for example you can resolve currencies from 
  a database or a webservice
* The default `RoundingMode` to use in monetary operations
* The decimal precision to use to intercept rounding errors (see `RoundingErrorDecimals`): by default
  all internal operations are rounded to 8 decimal places, but you can change this value up to 28 decimal places,
  for example if you need to perform operations with cryptocurrencies that typically have a lot of decimal places
* A log of operations that have generated rounding errors (according to the `RoundingErrorDecimals` value)

### Money

A `Money` is Moneta's monetary value data type built around a `decimal` value
tagged with a `Currency`.

### Currency

In Moneta a Currency is any instance of `Currency<TSelf>`. There's no particular
constraint of what is a valid currency: you're free to introduce your own currency
if they feet in your domain. The only constraint is that currencies with the same `Code` are 
treated as equivalent.

#### Currency Decimal Places
An important property of a currency is the number of decimal places it supports.
Whenever you perform a monetary operation, the result is rounded accordingly with a potential
rounding error.

### Rounding Error Detection

Moneta aims to support safe monetary calculus with a fluent algebraic API. The design goal
is to provide an API that allows you to perform monetary operations within a safe
context that detects and reports any potential rounding error and allows you to handle them
accordingly to your domain needs.

A *rounding error* occurs when an operation could not be performed without a loss of precision.
They are influenced not only by the *values* involved in the operation
but also by the `RoundingMode` and the `MonetaryContext.RoundingErrorDecimals` involved.

There are two main causes of rounding errors:
* Split Operations
* Floating Point Operations

##### Rounding Errors in Split Operations

Rounding errors in split operations occurs when you try to split a `Money` in parts that cannot be
fairly split accordingly to `RoundingMode` used.

For example, if you try to split 1.00 EUR in 3 parts, the result will be 0.33 EUR with a rounding error
of 0.01 EUR.

Other cases arises when you perform a *weighted* split operation using floating point weights.

##### Rounding Errors in Algebraic Operations

Rounding errors in algebraic operations occurs when you perform an operation between a `Money` and 
a floating point number with a decimal part that exceeds the decimal places supported by the `Currency` target.

For example, if you try to add 1.12345678 EUR to 1.12 EUR, the result will be 2.24 EUR with a rounding error
of 0.00345678 EUR.
 
### Safe and Unsafe Operations

Moneta's API provides two different kind of operations: *safe* and *unsafe* operations.

*Safe operations* are operations that returns to the caller the `error` of the operation and
alleviates the context from keeping track of the rounding errors. The caller is responsible
to handle the error accordingly to the domain needs.

On the other hand, *unsafe operations* are operations that doesn't return the `error` of the operation
and the context is responsible to keep track of the rounding errors.
The caller is responsible to check the context for any rounding error and handle them accordingly.   

### Currency System

#### Undefined Currency

Is a special currency identified by the Iso-4217 code `XXX` that is used to represent a `Money` without a currency.
You can create many variants of `UndefinedCurrency` with different `DecimalPlaces`.

#### Currency Compatibility and Binary Operations

Two `Currencies` are compatible if they have the same `Code` or one of them is `UndefinedCurrency`.

Binary operations between two `Money` instances are allowed only if their `Currencies` are compatible.



### Currency Providers

Moneta provides many extension points to customize the behavior of the library.

One of the most important is the `ICurrencyProvider` by witch you can integrate you currency system easily.

For example you can implement a `ICurrencyProvider` that resolves currencies from a database or a webservice.

Moneta provides the following providers:
* `NullCurrencyProvider` that doesn't resolve any currency
* `IsoCurrencyProvider` that resolves currencies from the Iso-4217 standard. This provider can be customized
  to resolve a subset of the standard.
* `ChainOfCurrencyProvider` that resolves currencies from a list of other providers, suitable for caching
  and fallback scenarios


### Exchange Rate Conversion

TBD

### Exchange Rate Providers

TBD
