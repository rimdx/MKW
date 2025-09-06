# Versioning

This article discuss major changes in the core functionality.  Each significant
change in the format or core functionality shall follow bump major version as
well.  Major versions will be as follows in early stages of development:
`alpha`, `beta`, `gamma`, etc. and continue as `v1`, `v2`, etc. after the
official release.

Throughout this article, this version will be referenced as "format version
`alpha`".

## Beta (0.2.0)

[unreleased]

Added administrator and trust control. More information is available in the
section [Admin](internal-design.md#admin).

Trust control prevents attacks in which the attacker modifies the database
by, for example, adding a blank new users, which someone will share a secret
to, following to its leakage.

When users unlocks the database, they automatically verify themselves using
their password, their administrator using their digital signature, and
determine which user they would trust or not based on administrator's
signature.

This version is not backward compatible to the version `alpha`.

## Alpha (0.1.0)

Initial version, consisting only [Users](internal-design.md#users) and
[Entries](internal-design.md#entries) first place entities.  This format
provides minimal multi-user encryption of the entries and basic user
management.

The format version `alpha` does not implement trust system, meaning it's not.
secure enough against fake users.  This was improved in the version `beta` with
introduction of [Admin](internal-design.md#admin).
