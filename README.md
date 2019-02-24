# RestNexus
Central REST distribution service written in C# using .NET Core WebAPI and Jint. It's licensed under the terms of the GNU General Public License, version 3 or later (GPLv3+).

Create endpoints available for other services to call into, write JavaScript to process the payload and forward the call to another service.

### Requirements
* Microsoft Visual Studio 2017 (Community Edition or higher to build)
* Workload for .NET Core cross-platform development
* NuGet to install referenced packages
* LibMan/Bower to install referenced client-side libraries

## Technical Details

### Components
RestNexus is fully written in C# and should work on all platforms that support .NET Core 2.1 or later.
- [ASP.NET Core WebAPI](https://docs.microsoft.com/en-us/aspnet/core/web-api/) as hosting/service infrastructure
- [Jint](https://github.com/sebastienros/jint) as JavaScript/ECMAScript engine to run transformation code
- [Monaco Editor](https://github.com/Microsoft/monaco-editor) as Web-based script editor
