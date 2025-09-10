# Cryptography API reference

## Abstract

The cryptography interface is defined in the `MKW.Cryptography` library.

To begin using the cryptography API, please reference the following assemblies:

- `MKW.Common`
- `MKW.Cryptography`
- `MKW.Cryptography.Loader`

## Initializing a provider

Please reference to the `MKW.Cryptography.Loader` library, which will
help loading the cryptography provider based on the platform and user
configuration.

Use one of the following code snippets to load a cryptography provider:

```cs
// Loads default cryptography provider (recommended).
var crypto = CryptographyLoader.GetProvider();

// Loads MKW.Cryptography.BouncyCastle cryptography provider.
var cryptoBouncyCastle = CryptographyLoader.GetProvider(BouncyCastleLoader.Name);

// Loads MKW.Cryptography.System cryptography provider.
// Note: won't work in .NET Framework projects.
var cryptoSystem = CryptographyLoader.GetProvider(SystemCryptographyLoader.Name);
```

## Using the API

Through the `ICryptographyProvider` interface, you can access the following
cryptographic operations:

- Symmetric operations
  - Key generation
  - Encryption
  - Decryption
- Asymmetric operations (public key cryptography)
  - Encryption
  - Decryption
  - Signature generation
  - Signature verification
- Secret key derivation from text password
- Random bytes generation

Follow the [ICryptographyProvider.cs](./MKW.Cryptography/ICryptographyProvider.cs)
file more.

## Implementing Custom Cryptography Providers

### 1. Create new class library project

Please name the project as `MKW.Cryptography.<CryptoLibrary>`.

Reference the following assemblies:

- `MKW.Common` (optional)
- `MKW.Cryptography` (the interfaces)
- `MKW.Cryptography.Loader` (TODO: to register loader)

### 2. Implement the provider

Create a class that implements the `MKW.Cryptography.ICryptographyProvider`
interface.  This is like the host of the provider.

Then add separate classes for each functionality, implementing the following
interfaces:

- `ISymmetricTransformer`
- `IAsymmetricPrivateTransformer`+`IAsymmetricPublicTransformer`
- `IUserCredentials`
- `IRandomGenerator`

For each class, make them hidden for external assemblies by changing class
modifier to internal, hide the constructor by changing its visibility to
`private` or `protected` and create static methods `Create` and `Open`
taking all the information needed to initialize the object.  The
`ICryptographyProvider` implementation should refer to these methods.

### 3. Register provider in the loader

This is not yet implemented in the loader, so currently the only option is to
manually modify the `MKW.Cryptography.Loader` library, and introducing
supporting the module.

### 4. Testing

The `MKW.Cryptography.Tests` project contains a bunch of test cases.  It
also checks compatibility between different modules to ensure the client will
work the same way no matter which crypto provider is currently in use.

Just modify the test matrix of the test fixtures, by adding yours.
