# Splunk Enterprise SDK for C# 
## Version 2.2.9

The Splunk Enterprise Software Development Kit (SDK) for C# contains library code and examples designed to enable developers to build applications using the Splunk platform.

The Splunk platform is a search engine and analytic environment that uses a distributed map-reduce architecture to efficiently index, search, and process large time-varying data sets.

The Splunk platform is popular with system administrators for aggregation and monitoring of IT machine data, security, compliance, and a wide variety of other scenarios that share a requirement to efficiently index, search, analyze, and generate real-time notifications from large volumes of time-series data.

The Splunk developer platform enables developers to take advantage of the same technology used by the Splunk platform to build exciting new applications.

For more information, see [Splunk Enterprise SDK for C#](https://dev.splunk.com/enterprise/docs/devtools/csharp/sdk-csharp/) on the Splunk Developer Portal.

## What's new in Version 2.x

Version 2.0 introduces new modern APIs that leverage the latest .NET platform advancements.

* Async. All APIs are 100% asynchronous supporting the new [async/await](http://msdn.microsoft.com/en-us/library/hh191443.aspx) features.
* All APIs follow .NET guidelines and abide by FxCop and StyleCop rules.
* Reactive Extensions. Splunk Enterprise query results implement [IObservable<T>](http://msdn.microsoft.com/library/dd990377), allowing usage with the [.NET Reactive Extensions](http://msdn.microsoft.com/data/gg577610).
* Support for cross-platform development. The Splunk API client (Splunk.Client.dll) in the new version is a [Portable Class Library](http://msdn.microsoft.com/library/vstudio/gg597391.aspx) supporting .NET development on multiple platforms.

Below is an example of a simple one shot search:

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

This SDK supports .NET 4.5/Mono 3.4, PCL (Windows 8.1, Windows Phone 8.1, iOS (via Xamarin.iOS), and Android (via Xamarin.Android) platforms.

## Compatibility

The Splunk Enterprise SDK for C# version 2.x is a rewrite of the 1.x SDK, and introduces completely new APIs.

>**Important**: Applications built with Splunk Enterprise SDK for C# version 1.x will not recompile using Splunk Enterprise SDK for C# version 2.xf.

Splunk Enterprise SDK for C# version 2.x includes a subset of the capability in version 1.0 of the SDK, and focuses on the most common customer scenarios. The major focus areas are search, search jobs, configuration, and modular inputs.

The areas that are covered are:
* Login
* Access control (users and passwords)
* Searches (normal, blocking, oneshot, and export)
* Jobs
* Reports (saved searches)
* Configuration and properties
* Indexes
* Inputs (sending simple and streamed events to Splunk Enterprise)
* Applications
* Modular inputs

See the [Splunk REST API Coverage](https://docs.google.com/spreadsheets/d/1lTlJ_z4tBpn_xPnJNDapiAxwdtQWvFGH31G6WIkMYwU/edit#gid=0) for details.

## Getting started with the Splunk Enterprise SDK for C# 

The Splunk Enterprise SDK for C# contains library code and examples that show how to programmatically interact with the Splunk platform for a variety of scenarios including searching, saved searches, data inputs, and many more, along with building complete applications.

### Requirements

Here's what you need to get going with the Splunk Enterprise SDK for C# version 2.x.

*  Splunk Enterprise
   
   If you haven't already installed Splunk Enterprise, download it [here](http://www.splunk.com/download). For more information, see the Splunk Enterprise [Installation Manual](https://docs.splunk.com/Documentation/Splunk/latest/Installation).
   
   The Splunk Enterprise SDK for C# has been tested with Splunk Enterprise 7.0 and 7.2.

*  Splunk Enterprise SDK for C# 
   
    *  Use the MyGet feed

        Download the Splunk SDK C# NuGet packages from [MyGet](http://www.myget.org). Add the following feed to your package sources in Visual Studio:
        `https://splunk.myget.org/F/splunk-sdk-csharp-pcl/`

        The following packages are in that feed:
        * **Splunk.Client**: Client for the Splunk Enterprise REST API. This library is portable.
        * **Splunk.ModularInputs**: Functionality for building modular inputs.
        
        >**Note**: Both packages are published to NuGet when the SDK is released.

    *  Get the source

        Download the [Splunk Enterprise SDK for C# ZIP file](https://github.com/splunk/splunk-sdk-csharp-pcl/archive/master.zip) and extract the contents. If you are want to contribute to the Splunk Enterprise SDK for C#, clone the repository from [GitHub](https://github.com/splunk/splunk-sdk-csharp).

### Developer environments

The Splunk Enterprise SDK for C# supports development in the following environments:

*   [Microsoft Visual Studio](http://www.microsoft.com/visualstudio/downloads) 2012 and later

    You must also install [Code Contracts for .NET](http://visualstudiogallery.msdn.microsoft.com/1ec7db13-3363-46c9-851f-1ce455f66970). 
    
    >**Note**: Close Visual Studio before installing Code Contracts. Otherwise, the installation will not work, despite appearing to.
    
    To run the unit tests, install an [xUnit](https://github.com/xunit/xunit) runner. If you use resharper, install its [xUnit.net Test Support](https://resharper-plugins.jetbrains.com/packages/xunitcontrib/1.6.2). Otherwise, install the [xUnit.net runner for Visual Studio 2012 and 2013](http://visualstudiogallery.msdn.microsoft.com/463c5987-f82b-46c8-a97e-b1cde42b9099).

*  Xamarin Studio and Mono Develop

    You must set the MonoCS compilation constant in the project settings for **Splunk.Client.csproj** and **Splunk.ModularInputs.csproj**.

    To run the unit tests, download [xUnit](https://github.com/xunit/xunit/releases) and run it using the command-line tools or GUI runner.


#### Build the SDK

To build the SDK, the examples, and the unit tests after extracting or cloning the SDK:

1. At the root level of the **splunk-sdk-csharp-pcl** directory, open the **splunk-sdk-csharp-pcl.sln** file in Visual Studio.
2. On the **BUILD** menu, click **Build Solution**.

#### Examples and unit tests

The Splunk Enterprise SDK for C# includes full unit tests that run using [xunit](https://github.com/xunit/xunit) as well as several examples.


#### Solution layout

| Directory                        | Description                                                                  |
|:-------------------------------- |:---------------------------------------------------------------------------- |
| /src                             |                                             |
|     ..Splunk.Client                  | Client for the Splunk Enterprise REST API.  |
|     ..Splunk.ModularInputs           | Functionality for building modular inputs.  |
|     ..Splunk.Client.Helpers          | Helper utilities used by tests and samples. |
| /examples                        |                                             |
| ..Windows8/Search                | Contains a Windows Store Search App. |
| ..authenticate                   | Connects to a Splunk Enterprise instance and retrieves a session token. |
| ..list_apps                      | Lists installed applications on a Splunk Enterprise instance. |
| ..mock-context                   | Demonstrates how to use the included HTTP record/play framework for unit tests. |
| ..mock-interface                 | Demonstrates how to mock the functional interface for Splunk Enterprise entities. |
| ..mock-object                    | Demontrates how to mock concrete SDK objects and create fake HTTP responses for unit tests. |
| ..normal-search                  | Performs a normal search against a Splunk Enterprise instance and retrieves results using enumeration and Rx. |
| ..random-numbers                 | Sample modular input that returns randomly-generated numbers. |
| ..saved-searches                 | Creates a saved search and retrieves results.       |
| ..search-export                  | Creates a search and uses the Export endpoint to return results. |
| ..search-realtime                | Creates a real-time search.                         |
| ..search-response-message-stream | Demonstrates how to run long-running search jobs and use **Job.GetSearchReponseMessageAsync**. |
| ..search                         | Performs a oneshot search.                          |
| ..submit                         | Creates an index, then sends events over HTTP to it |
| /tests                           |                                                     |
| ..unit-tests                     | Contains unit tests for all of the classes in the SDK. Does not require a Splunk Enterprise instance. |
| ..acceptance-tests               | Contains end-to-end tests using the SDK. By default, these tests run against a Splunk Enterprise instance. You can also run tests in playback mode by setting **MockContext.Mode** to "Playback" in **App.Config**. |

### Changelog

The [CHANGELOG](CHANGELOG.md) contains a description of changes for each version of the SDK. For the latest version, see the [CHANGELOG.md](https://github.com/splunk/splunk-sdk-csharp-pcl/blob/master/CHANGELOG.md) on GitHub.

### Branches

The **master** branch represents a stable and released version of the SDK.
To learn about our branching model, see [Branching Model](https://github.com/splunk/splunk-sdk-csharp-pcl/wiki/Branching-Model) on GitHub.

## Documentation and resources

| Resource                | Description |
|:----------------------- |:----------- |
| [Splunk Developer Portal](http://dev.splunk.com) | General developer documentation, tools, and examples |
| [Integrate the Splunk platform using development tools for .NET](https://dev.splunk.com/enterprise/docs/devtools/csharp)| Documentation for .NET development |
| [Splunk Enterprise SDK for C# Reference](https://docs.splunk.com/Documentation/CshrpSDK) | SDK API reference documentation |
| [REST API Reference Manual](https://docs.splunk.com/Documentation/Splunk/latest/RESTREF/RESTprolog) | Splunk REST API reference documentation |
| [Splunk>Docs](https://docs.splunk.com/Documentation) | General documentation for the Splunk platform |
| [GitHub Wiki](https://github.com/splunk/splunk-sdk-csharp-pcl/wiki/) | Documentation for this SDK's repository on GitHub |


## Community

Stay connected with other developers building on the Splunk platform.

* [Email](mailto:devinfo@splunk.com)
* [Issues and pull requests](https://github.com/splunk/splunk-sdk-csharp-pcl/issues/)
* [Community Slack](https://splunk-usergroups.slack.com/app_redirect?channel=appdev)
* [Splunk Answers](https://community.splunk.com/t5/Splunk-Development/ct-p/developer-tools)
* [Splunk Blogs](https://www.splunk.com/blog)
* [Twitter](https://twitter.com/splunkdev)

### Contributions

If you would like to contribute to the SDK, see [Contributions to Splunk](https://www.splunk.com/en_us/form/contributions.html).

### Support

*  You will be granted support if you or your company are already covered under an existing maintenance/support agreement. Submit a new case in the [Support Portal](https://www.splunk.com/en_us/support-and-services.html) and include "Splunk Enterprise SDK for C# PCL" in the subject line.

   If you are not covered under an existing maintenance/support agreement, you can find help through the broader community at [Splunk Answers](https://community.splunk.com/t5/Splunk-Development/ct-p/developer-tools).

*  Splunk will NOT provide support for SDKs if the core library (the code in the <b>1. src</b> directory) has been modified. If you modify an SDK and want support, you can find help through the broader community and [Splunk Answers](https://community.splunk.com/t5/Splunk-Development/ct-p/developer-tools). 

   We would also like to know why you modified the core library, so please send feedback to _devinfo@splunk.com_.

*  File any issues on [GitHub](https://github.com/splunk/splunk-sdk-csharp-pcl/issues).

### Contact Us

You can reach the Splunk Developer Platform team at _devinfo@splunk.com_.

## License

The Splunk Enterprise Software Development Kit for C# is licensed under the Apache License 2.0. See [LICENSE](LICENSE) for details.
