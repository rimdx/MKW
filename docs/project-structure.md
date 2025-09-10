# Multi-Key Wallet project structure

Multi-Key Wallet implementation consists several independent projects,
interacting with each other through exported APIs.  This article will
go through their structure, relationships, and responsibility.

## Cryptography Backend

We maintain an extensible system for cryptographic operations, allowing
several independent cryptography providers to take place, based on user
configuration, platform, etc.

### Interface

The cryptography provider API along with several common helpers are defined
in the `MKW.Cryptography` library.  Each cryptography provider should
reference this assembly, and implement the `MKW.Cryptography.ICryptographyProvider`
interface.

Currently, we have the following cryptography providers:

### Bouncy Castle

The `MKW.Cryptography.BouncyCastle` library implements the abstract
cryptography provider using the Bouncy Castle library.

### .NET Cryptography

The `MKW.Cryptography.System` library implements the cryptography
provider using built-in .NET Cryptography API, available in the
`System.Security.Cryptography` namespace.

However, this does exist only in .NET core projects, and is quite limited
on several platforms.  This cryptography provider is not available in .NET
framework projects.  Please consider using Bouncy Castle provider instead.

### The Loader

When referencing cryptography providers from outside, please consider using
the `MKW.Cryptography.Loader` library, which will handle all platform
specific logic and produce an abstract `ICryptographyProvider` based on user's
needs.

### API

Please refer to the [api-crypto.md](api-crypto.md) article for more.

## Storage Backend

## The Client Library
