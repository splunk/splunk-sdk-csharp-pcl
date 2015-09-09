# Splunk SDK for C# PCL

## Version 2.2.0

### New features and APIs
* Added support for cookie-based authentication, for Splunk 6.2+.

### New examples
* `search-response-message-stream`: Shows how to get an `HttpResponseMessage` for any `ExecutionMode`.

### Minor Changes
* Added `Job.GetSearchResponseMessage` which returns an `HttpResponseMessage` for a given `Job`.
* Removed redundant StyleCop package. Pull Request #40.

## Version 2.1.2

## Bug fixes
* Search and job operations now default to a count of 0, which represents all.

## New examples
* `Get-SplunkDataInputs`: Shows how to access an unsupported REST API endpoint.

## Minor changes
* The GitHub commits example now uses the Json.NET library instead of a dictionary.
* Modular input validation now logs exceptions as `INFO` instead of `DEBUG`.
* Added xunit.runner to the `packages` directory.

## Version 2.1.1

### Fixes
* Removed `All` and `Scheme` attach points for debugging Modular Inputs.
* Hardened logging for Modular Inputs:
  * Added more contextual information.
  * Ensured all exceptions are captured and logged.

## Version 2.1.0

### New Features
* Add the GitHub commits modular input example.
* Add support for Xamarin Forms.
* Add support for remotely debugging modular inputs with Visual Studio.

### Minor changes
* Building a modular input example no longer creates its `.spl` file.
* `Job` returns all available results by default if no arguments are passed to `Job.GetSearchResultsAsync()`, `Job.GetSearchPreviewAsync()`, and `Job.GetSearchEventsAsync()`.
* `Service` now uses interfaces in place of implementations in order to improve testability.
* Add a `ModularInputs/EventWriter.LogAsync()` overload that takes a `Severity` enum as a parameter.
* Add a `Service` constructor that takes a `Uri` as a parameter.

## Version 2.0.0
* Add ConfigureAwait(false) to all await calls in the library to prevent some deadlocks in user code.
* Add User-Agent string to HTTP requests.

## Version 2.0.0 pre-release

* Initial release.
