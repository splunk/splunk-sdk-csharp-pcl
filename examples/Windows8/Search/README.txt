This is a Windowns 8.1 store app example implementing splunk search using Splunk C# SDK 2.0.
The app requires Microsoft Visual Studio 2013 (update 2), Windows Phone 8.1 SDK Integration nuget package and Behaviors SDK (XAML) nuget package.

To run the app in a develop environment that doesn't install proper certificate, may need to config your splunkd service to disable SSL: edit file at $SPLUNK_HOME\etc\system\default\server.conf, change enableSplunkdSSL = false

This app will not work against a default install of Splunk 6.2 even with signed certificates,
since Windows 8.1 does not allow the .NET HttpClient the SDK uses internally
to set the default SSL handshake to use SSLv3, which Splunk 6.2 requires. You can set
requireSsl3 = false in $SPLUNK_HOME\etc\system\default\server.conf to disable this behavior
and allow the example to work, but you should not do this in production.