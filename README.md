# Lurgle.Alerting

[![Version](https://img.shields.io/nuget/v/Lurgle.Alerting?style=plastic)](https://www.nuget.org/packages/Lurgle.Alerting)
[![Downloads](https://img.shields.io/nuget/dt/Lurgle.Alerting?style=plastic)](https://www.nuget.org/packages/Lurgle.Alerting)
[![License](https://img.shields.io/github/license/MattMofDoom/Lurgle.Alerting?style=plastic)](https://github.com/MattMofDoom/Lurgle.Alerting/blob/dev/LICENSE)

[FluentEmail](https://github.com/lukencode/FluentEmail) is a fantastic open source email solution, which can be configured in innumerable ways. Anyone needing an email solution can add Fluent Email and get started.

**Lurgle.Alerting** is an implementation of FluentEmail that can help to save time in getting up and running, and provides some useful functionality.

- Includes MailKit and SmtpClient implementations, with MailKit as default, and config options to switch and configure
- Integrated SMTP mail host connectivity test
- Includes Razor, Liquid (Fluid), and Handlebars template functionality
- Implemented with fluent methods that complement and extend FluentEmail methods
- Implements default mail from and mail to addresses, allowing quick alerts to be implemented
- Implements an ability to retrieve email address from config files
- Implements ability to retrieve email template filenames from config files
- Implements a default email template filename format (alert{0}.html)
- Implements a debug flag for emails that allows substitution of any recipient field with the default MailTo address - can help prevent information leakage or inadvertent outbound emails
- Adds optional string formatting with parameters to subject and message body
- Optionally add calling method property to email
- Configuration via app config or passing a config class to the library
  - Default settings supplied where it's sane to do so, allowing only essential properties to be configured

