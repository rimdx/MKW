# Quick Start

This article explains basic concepts and guides how to begin using the
Multi-Key Wallet application for yourself.

## Get Multi-Key Wallet client

We provide signed binaries of our official client to work with Multi-Key Wallet
databases.  You can either use a fully-functioning GUI application or the
command-line tool.  Both can be found on the [GitHub Release](https://github.com/rimdx/MKW/releases/latest)
age.  For most of the cases, the graphical client would be recommended.

[TODO] An installer will be added in the near future.

Download the executable and put it into an accessible location, optionally
accessible from PATH.

## Create Database

In the GUI client, click File->New.  Choose location and name of the database,
and click 'Save'.

Then the application will prompt the master secret key.  It will encrypt the
database, as the administrator user.  Choose strong and memorable password,
since it cannot be reset or may be used by an attacker to steal your data or
gain privileges in the database.

<!-- If using the command-line client, the following command will initialize the
database in the exact same way:

```pwsh
mkw create <FILE>
# it will prompt the password
``` -->

## Create Entries

The entries are encrypted in the Multi-Key Wallet database, meaning only
authorized users can access them.

To create an entry, locate the 'Entries' page in the menu on the left-hand side
of the window, assuming if database is opened.  Please refer to [Create Database](#create-database) or
[Opening an Existing Database](#opening-an-existing-database) if one cannot be
found.

Then, in the context menu click 'New Entry' option.  This will open a dialog,
in which the content can be edited.  Click 'OK' to confirm creation.

To edit entry, click 'Edit Entry' option in the context menu, or, simply
double click the entry.

## Create Users

Before creating users, please familiarize yourself with the concepts behind.

Each user is encrypted with their own personal secret passwords, which they
enter each time unlocking the database.  They should not share this password
with anyone, even the administrators should not see it.

However, even if the user is initialized, the rest still couldn't share the
entries with them -- the admin must verify their user before.

Here is an approximate sequence of actions required to initialize a blank new
user.  This requires a few interactions from the new user and the admin,
alongside with exchanging the database between the agents.

<!-- TODO: Bob-Alice naming? -->

1. The user creates their profile.
2. The database is shared with the admin (for more see [Sharing Database](#sharing-database)).
3. User sends their fingerprint to the admin through a trusted communication
   channel, for example, email.
4. Admin then updates the database and receives the fingerprint. Then may
   verify the user's profile. This will also share all the entries in the
   database.
5. The database is sent to the other users using the shared location.

## Sharing Database

The basic concept of Multi-Key Wallet is that its database can be safely shared
between users and collaborators.  Due to the encryption, it can even be stored
on public locations, assuring data safety.

For example, the database could be stored in the following sites:

- Subversion Repository
- Git Repository
- OneDrive-Synced Folder
- Shared Network Drive

Make sure that the users have write access to this file (on shared location),
even if they are not allowed to create or edit entries.  It is required to
initialize their profiles.  Later on, read access would be enough.

## Opening an Existing Database

Click File->Open and select the database file.  Then the application will
prompt for your personal credentials.  Find yourself in the combobox and
enter your password.  If the challenge succeeds and the file is valid, the
database content will appear on the window.
