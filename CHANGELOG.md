# Splunk SDK for C# PCL

## Version 2.2.2

### Bug Fixes

* Fixed code contracts runtime errors caused by MyGet during the previous 2 releases. See GitHub issue #42.

## Version 2.2.1

### New features and APIs

* Implemented `SearchResult.Tags` property for search tags. GitHub issue #36.
* Added the `Job.GetSearchResponseMessageAsync` which enables you to fetch raw search results in a specific format: JSON, CSV, or any of several flavors of XML.
* Add the `search-response-message-stream` example which shows you how to execute long-running search jobs and how to use `Job.GetSearchReponseMessageAsync`.

### Bug Fixes

* Fixed code contracts runtime errors. GitHub issue #42.
* Fixed missing millisecond component when writing modular input timestamps. GitHub pull request #41.
* Restored compatibility for some examples with .Net v4.5.0.

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
