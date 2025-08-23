# hoohub

## Contents

- [About](#about)
- [Features](#features)
- [Setup and debugging](#setup-and-debugging)
   * [Requirements](#requirements)
   * [Configuration](#configuration)
   * [Working with migrations](#working-with-migrations)
   * [Working with emails](#working-with-emails)
   * [Javascript/CSS minificiation](#javascriptcss-minificiation)
- [Deployment process](#deployment-process)

---

## About

_HooHub_ is a .NET Razor Pages web application for the purpose of publishing web comics about Athena: the life of a little owl who (understandably) hates just about everything!

---

## Features

- View, link and like comics with no registration required
- Browse and query comics in the Archive
- As an authorised user, log in and submit new comics, manage older comics, schedule comic releases, view application events and more
- Secure two-factor authentication
- Full credit attributions

---

## Setup and debugging

### Requirements

- [.NET framework 9](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [PostGres 13+](https://www.postgresql.org)

---

### Configuration

Use the relevant `appsettings.json` file to configure _HooHub_:

- AppSettings:
    - TrustedLocations: IP addresses for which two-factor should not apply. This should only consist of the loopback address when running _HooHub_ locally!
    - ArtistCredit: The email address of the HooHubUser to link for artist credit on the About page
    - WriterCredit: The email address of the HooHubUser to link for writer credit on the About page
- AccountSettings: Settings for the initial accounts to be set up when _HooHub_ is initially started, or database recreated
    - Email: Email address for login
    - Handle: Handle for public display
    - SocialLink: Social media link for public display
- SmtpSettings:
    - Host: The host address to use for SMTP activity.
    - Port: The network port to use for SMTP activity.
    - SenderName: The sender name to use for SMTP activity.
    - SenderAddress: The sender address to use for SMTP activity.
    - Username: The username to use as part of the SMTP credentials.
    - Domain: The domain to use as part of the SMTP credentials.
    - EnableSsl: Whether to use SSL for the connection to the target SMTP server.
    - EnableEmails: Whether to send emails or not globally. This will prevent 2FA logins, so an account must have 2FA disabled to login if this is enabled.

The following environment variables are **required**:

- ASPNETCORE_ENVIRONMENT (Release/Development): The environment of the app when running.
- SMTP_PASSWORD: The SMTP password for the SMTP email server, as given in the appsettings.
- DATABASE_URL: The URL for the PostGres instance to connect to.

---

### Working with migrations

_HooHub_ makes use of Entity Framework to handle database operations; if you wish to make changes to the data structure of the application then migrations will be necessary.

Use `Add-Migration YourMigrationName` in the Package Manager Console to add a migration containing code to handle data changes whenever you make modifications to the data classes (data type changes, new fields, removed fields, new classes). When _HooHub_ is run, migrations will be handled automatically: **always double check and verify the generated migration files and backup databases before running HooHub when migrating.**

Note that if adding new classes to be tracked using EF, you will need to update the `HooHubContext` class responsible for handling SQL connectivity and mapping between tables.

---

### Working with emails

_HooHub_ uses the MJML framework to build emails for templating: when creating a new email, use MJML and build your email off an existing MJML template. Ensure that any newly created emails have a corresponding default template created during the startup checks under `Program.cs`.

It is recommended to use the [official MJML plugin for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=attilabuti.vscode-mjml) when working with email templates.

The MJML template files can be found under the _templates_ folder.

---

### Javascript/CSS minificiation

_HooHub_ makes use of the [LigerShark WebOptimizer](https://github.com/ligershark/WebOptimizer) library to minify all non-bootstrap/lib CSS and JS files.

Minification options can be modified under `Program.cs`.

---

## Deployment process

HooHub is managed via [Digital Ocean](https://www.digitalocean.com/).