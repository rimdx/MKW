# Resource Managing Guidelines

## Disposable

Each object, unless an edge case, should implement `System.IDisposable`
interface, even if no unmanaged resources were explicitly used -- for
style consistency we will still consider disposing them in a chain.

## Interfaces

Every object should implement a consistent amount of functionality throughout
the internal API.  This is achieved by declaring an `interface` for each of
abstraction considered useful for the code.  For consistent code-style, the
interfaces also inherit `System.IDisposable` interface.

As an example, let's go though details of the `MKW.Core.Client.ITrustProvider`
interface;  It declares an object that can do every kind of operation for
accessing trust of an entity, like a user.

The declaration at the moment of me writing this article:

```cs
public interface ITrustProvider : IDisposable
{
    IEnumerable<UserInfo> EnumerateImplicitlyTrustedUsers();
    IEnumerable<UserInfo> EnumerateExplicitlyTrustedUsers();

    Trust GetExplicitTrust(ReadOnlySpan<byte> publicKey);
    Trust GetImplicitTrust(UserId userId);
}
```

Nothing really special in here;  Please consider maintaining this style in
the rest of the codebase.

## Inheritance

Don't use this feature, related patterns, and abstract classes unless really
needed.

## Modularity and forwarding

Each class in the core libraries should implement a single part of functionality,
for example, a basic trust provider (`UserTrustProvider`), which calculates trust
for a specific users provides a specific atomic level of contribution to the entire
client.  It implements the `ITrustProvider` interface.  The higher level of client
may also wish to provide `ITrustProvider` to the public (for example, `UserSession`).
It should consider re-implement the `ITrustProvider` interface along with the rest,
maintain their own instance of `UserTrustProvider : ITrustProvider`, wrapping all
the methods.  Don't be afraid of duplicating code, we've got `alt+enter` in the VS!

Keep each abstraction simple and atomic, following their own level in the client!
