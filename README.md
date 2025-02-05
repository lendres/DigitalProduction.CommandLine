# DigitalProduction.CommandLine
Command line argument parsing library for .Net.

# Legacy
## Origin
This library is derived directly from the Plossum command line parsing library written by Peter Palotas.
- [Code project article and original source code.](https://www.codeproject.com/Articles/19869/Powerful-and-simple-command-line-parsing-in-C)
- [NuGet package](https://www.nuget.org/packages/Plossum.CommandLine)

## Issues
The NuGet package was available and functional, so the software was usable.  However, there were a few issues.
- The code was based on outdated an outdated framework.  This was causing compile time warnings.  
- The code base was not being maintained.
- The source code was only available as a download (it was static).  There wasn't a repository to which to contribute changes.
- The examples were not directly runnable.
- Some data types contained errors.
- Nullable types were not supported.

# New Library
## Justification
This effort was initiated to achieve several goals:
- Remove compile time warnings from using legacy frameworks.
- Provide a GitHub repository.
- Update the code base to use modern language features.
- Simplify the code base.
- Remove bugs and support Nullable types.

## Changes
- The library name was changed to avoid confusion with the original.
- The namespaces were consolidated and changed to match the library name.
- Updates to reflect new library features.
- Simplification of code.
- Added unit tests.
- Added a .Net Maui example.
