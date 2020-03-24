# Splunk SDK for C# 
## Version 2.2.9

> **Note:** For the most up to date documentation for the Splunk SDK for C#, 
> see http://dev.splunk.com/csharp.

The Splunk Software Development Kit (SDK) for C# contains library code and
examples designed to enable developers to build applications using Splunk.

Splunk is a search engine and analytic environment that uses a distributed
map-reduce architecture to efficiently index, search, and process large
time-varying data sets.

The Splunk product is popular with system administrators for aggregation and
monitoring of IT machine data, security, compliance and a wide variety of
other scenarios that share a requirement to efficiently index, search, analyze,
and generate real-time notifications from large volumes of time series data.

The Splunk developer platform enables developers to take advantage of the
same technology used by the Splunk product to build exciting new applications
that are enabled by Splunk's unique capabilities.

## What's new in Version 2.x

Version 2.0 introduces new modern APIs that leverage the latest .NET platform advancements.

* Async - All APIs are 100% asynchronous supporting the new [async/await](http://msdn.microsoft.com/en-us/library/hh191443.aspx) features.
* All APIs follow .NET guidelines and abide by FxCop and StyleCop rules.
* Reactive Extensions - Splunk Enterprise query results implement [IObservable<T>](http://msdn.microsoft.com/library/dd990377), allowing usage with the [.NET Reactive Extensions](http://msdn.microsoft.com/data/gg577610).
* Support for cross-platform development - The Splunk API client (Splunk.Client.dll) in the new version is a [Portable Class Library](http://msdn.microsoft.com/library/vstudio/gg597391.aspx) supporting .NET development on multiple platforms.

Below is an example of a simple One Shot Search:

```csharp
using Splunk.Client;

var service = new Service(new Uri("https://localhost:8089"));

//login
await service.LogOnAsync("admin", "changeme");

//create a One Shot Search and retrieve the results
var searchResults = await service.SearchOneShotSearchAsync("search index=_internal | head 10");

//loop through the results
foreach (var result in searchResults)
{
    //write out the raw event
    Console.WriteLine(string.Format("{0:D8}: {1}", ++recordNumber, result.GetValue("_raw")));
}
```

## Supported platforms

.NET 4.5/Mono 3.4, PCL (Windows 8.1, Windows Phone 8.1, iOS (via Xamarin.iOS), Android (via Xamarin.Android)

## Compatibility

The Splunk SDK for C# version 2.x is a rewrite of the existing SDK, and introduces completely new APIs.

__Important: Applications built with Splunk SDK for C# version 1.x will not recompile using Splunk SDK for C# version 2.xf.__

Splunk SDK for C# version 2.x includes a subset of the capability in version 1.0 of the SDK, and focuses on the most common scenarios that we have seen customers using. The major focus areas are _search_, _search jobs_, _configuration_, and _modular inputs_.

Following is a breakdown of the areas covered:

* Login
* Access control (users and passwords)
* Searches (normal, blocking, oneshot, and export)
* Jobs
* Reports ("saved searches" in Splunk Enterprise 5)
* Configuration and Config Properties
* Indexes
* Inputs (sending simple and streamed events to Splunk Enterprise)
* Applications
* Modular inputs

For detailed API coverage, see this [coverage matrix](https://docs.google.com/spreadsheets/d/1lTlJ_z4tBpn_xPnJNDapiAxwdtQWvFGH31G6WIkMYwU/edit#gid=0).

We will publish guidance on how to migrate applications built using the Splunk SDK for C# 1.x to use the Splunk SDK for C# 2.x.

## Getting started with the Splunk SDK for C# 

The Splunk SDK for C# contains library code and examples that show how to
programmatically interact with Splunk for a variety of scenarios including
searching, saved searches, data inputs, and many more, along with building
complete applications.

The information in this Readme provides steps to get going quickly. In the
future we plan to roll out more in-depth documentation.

### Requirements

Here's what you need to get going with the Splunk SDK for C# version 2.x.

#### Splunk Enterprise

If you haven't already installed Splunk Enterprise, download it at
<http://www.splunk.com/download>. For more information about installing and
running Splunk Enterprise and system requirements, see the
[Splunk Installation Manual](http://docs.splunk.com/Documentation/Splunk/latest/Installation). The Splunk SDK for .NET has been tested with Splunk Enterprise 7.0 and 7.2.

#### Developer environments

The Splunk SDK for C# supports development in the following environments:

##### Visual Studio
The Splunk SDK for C# supports development in [Microsoft Visual Studio](http://www.microsoft.com/visualstudio/downloads) 2012 and later

You will need to install [Code Contracts for .NET](http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970)
(be sure to close Visual Studio before you install it or the install will not work, despite appearing to).

To run the unit tests you will need to install an [xUnit](https://github.com/xunit/xunit) runner:
* If you use resharper, install its [xUnit.net Test Support](https://resharper-plugins.jetbrains.com/packages/xunitcontrib/1.6.2).
* Otherwise, install the [xUnit.net runner for Visual Studio 2012 and 2013](http://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099).

##### Xamarin Studio / Mono Develop
The Splunk SDK for C# support development in Xamarin Studio and Mono Develop. You will need to set the __MonoCS__ complitation constant in the project settings for Splunk.Client.csproj and Splunk.ModularInputs.csproj.

To run the unit tests you will need to [download](https://github.com/xunit/xunit/releases) the latest release of xUnit and run using the command line tools or GUI runner.

### Splunk SDK for C# 

#### MyGet feed

Before the intial release, you can download the Splunk SDK C# NuGet packages from [MyGet](http://www.myget.org). Add the following feed to your package sources in Visual Studio: https://splunk.myget.org/F/splunk-sdk-csharp-pcl/

The following packages are in that feed:
* Splunk.Client - Client for Splunk's REST API. This is a portable library.
* Splunk.ModularInputs - Functionality for building Modular Inputs.

*Note*: Both packages will be published to NuGet when the SDK releases.

#### Getting the source

[Get the Splunk SDK for C#](https://github.com/splunk/splunk-sdk-csharp-pcl/archive/master.zip). Download the ZIP file and extract its contents.

If you are interested in contributing to the Splunk SDK for C#, you can
[get it from GitHub](https://github.com/splunk/splunk-sdk-csharp) and clone the
resources to your computer.

#### Building the SDK

To build from source after extracting or cloning the SDK, do the following"

1. At the root level of the **splunk-sdk-csharp-pcl** directory, open the
**splunk-sdk-csharp-pcl.sln** file in Visual Studio.
2. On the **BUILD** menu, click **Build Solution**.

This will build the SDK, the examples, and the unit tests.

#### Examples and unit tests

The Splunk SDK for C# includes full unit tests which run using [xunit](https://github.com/xunit/xunit) as well as several examples.

#### Solution Layout

The solution is organized into `src`, `examples` and `tests` folders.

##### src
* `Splunk.Client` - Client for Splunk's REST API.
* `Splunk.ModularInputs` - Functionality for building Modular Inputs.
* `Splunk.Client.Helpers` - Helper utilities used by tests and samples.

##### examples
* `Windows8/Search` - Contains a Windows Store Search App.
* `authenticate` - Connects to a Splunk Instance and retrieves Splunk's session token.
* `list_apps` - Lists installed applications on a Splunk instance.
* `mock-context` - Demonstrates how to use the included HTTP record/play framework for unit tests.
* `mock-interface` - Demonstrates how to mock the functional interface for Splunk entities.
* `mock-object` - Demontrates how to mock concrete SDK objects and fake out HTTP responses for unit tests.
* `normal-search` - Performs a normal search against a Splunk instance and retrieves results using both enumeration and with Rx.
* `random-numbers` - Sample modular input which returns a randoml generated numbers.
* `saved-searches` - Creates a new saved search and retrieves results.
* `search-export` - Creates a search and usings splunk's Export endpoint to push back results.
* `search-realtime` - Creates a realtime search.
* `search-response-message-stream` - Demonstrates how to execute long-running search jobs and how to use `Job.GetSearchReponseMessageAsync`.
* `search` - Performs a One Shot search.
* `submit` - Creates an index and then sends events over HTTP to that index

##### tests
* unit-tests - Contains unit tests for all of the classes in the SDK. Does not require a Splunk instance.
* acceptance-tests - Contains end to end tests using the SDK. These tests by default will go against a Splunk instance. Tests can also be run in playback mode by setting `MockContext.Mode` to `Playback` in `App.Config`.

### Changelog

The **CHANGELOG.md** file in the root of the repository contains a description
of changes for each version of the SDK. You can also find it online at
[https://github.com/splunk/splunk-sdk-csharp/blob/master/CHANGELOG.md](https://github.com/splunk/splunk-sdk-csharp/blob/master/CHANGELOG.md).

### Branches

The **master** branch always represents a stable and released version of the SDK.
You can read more about our branching model on our Wiki at
[https://github.com/splunk/splunk-sdk-csharp/wiki/Branching-Model](https://github.com/splunk/splunk-sdk-java/wiki/Branching-Model).

## Documentation and resources

If you need to know more:

* For all things developer with Splunk, your main resource is the [Splunk
  Developer Portal](http://dev.splunk.com).

* For more about the Splunk REST API, see the [REST API
  Reference](http://docs.splunk.com/Documentation/Splunk/latest/RESTAPI).

* For more about about Splunk in general, see [Splunk>Docs](http://docs.splunk.com/Documentation/Splunk).

## Community

Stay connected with other developers building on Splunk.

<table>

<tr>
<td><em>Email</em></td>
<td><a href="mailto:devinfo@splunk.com">devinfo@splunk.com</a></td>
</tr>

<tr>
<td><em>Issues</em>
<td><a href="https://github.com/splunk/splunk-sdk-csharp-pcl/issues/">
https://github.com/splunk/splunk-sdk-csharp/issues</a></td>
</tr>

<tr>
<td><em>Answers</em>
<td><a href="http://splunk-base.splunk.com/tags/csharp/">
http://splunk-base.splunk.com/tags/csharp/</a></td>
</tr>

<tr>
<td><em>Blog</em>
<td><a href="http://blogs.splunk.com/dev/">http://blogs.splunk.com/dev/</a></td>
</tr>

<tr>
<td><em>Twitter</em>
<td><a href="http://twitter.com/splunkdev">@splunkdev</a></td>
</tr>

</table>

### Contributions

If you want to make a code contribution, go to the
[Open Source](http://dev.splunk.com/view/opensource/SP-CAAAEDM)
page for more information.

### Support


1. You will be granted support if you or your company are already covered
   under an existing maintenance/support agreement. Submit a new case in the [Support Portal][contact] and include "Splunk SDK for C# PCL" in the subject line.

2. If you are not covered under an existing maintenance/support agreement, you
   can find help through the broader community at:

   <ul>
   <li><a href='http://splunk-base.splunk.com/answers/'>Splunk Answers</a> (use
    the <b>sdk</b> and <b>csharp</b> tags to identify your questions)</li>
   </ul>
3. Splunk will NOT provide support for the extension if the core library (the code in the
   '1. src' directory) has been modified.
   If you modify an SDK and want support, you can find help through the broader
   community and Splunk answers (see above). We would also like to know why you modified
   the code&mdash;please send feedback to _devinfo@splunk.com_.
4. File any issues on [GitHub](https://github.com/splunk/splunk-sdk-csharp-pcl/issues).

### Contact Us

You can [contact support][contact] if you have Splunk related questions.

You can reach the Dev Platform team at devinfo@splunk.com.

## License

The Splunk SDK for C# is licensed under the Apache License 2.0. Details can be
found in the LICENSE file.

[contact]:                  https://www.splunk.com/en_us/support-and-services.html
