---
_layout: landing
---

# About Bodoconsult.AppInfrastructure repository

>   [Licenses](LICENSE.md)

>   [Code of conduct](CODE_OF_CONDUCT.md)

>   [Contributing](CONTRIBUTING.md)


Bodoconsult.AppInfrastructure is a repository containing multiple libraries with basic features for writing application like console apps, WinForm apps or background service apps hosting a GRPC service or not.

The following basic features are for all types of service based, console based or desktop UI based applications:

-   Structured app start process with loading configuration from appsettings.json, setting up logging, DI container handling etc. (see IAppBuilder)

-   App logging (see IAppLoggerProxy)

-   App benchmarking for finding performance bottlenecks in an app (see IAppBenchProxy)

-   Business transactions for simplifying the communication with clients in a client server environment (see IBusinessTransactionManager)

-   Client notifications for simplifying the communication with clients in a client server environment (see IClientManager)

-   App performance measurement (APM) based on the [Microsoft Diagnostics](https://learn.microsoft.com/en-us/dotnet/core/diagnostics/) for usage with professionell APM tools like Application Insights, dotnet-counters and dotnet-monitor

-   Toast notifications to inform user via OS notifications (see IToastMessagingService)

-   Generating structured PDF documents (see PdfBuilder and PdfCreator classes)

-   Create structured documents and export them to PDF, XPS, WPF, HTML, RTF etc. (see IDocumentBuilder).

-   A set of extension methods for basic and more enhanced classes: 
    
    -   BoolExtensions

    -   DoubleExtensions

    -   FiletimeExtensions
    
    -   IntExtensions

    -   GuidExtensions

    -   StringExtensions

    -   TaskExtensions



This repository contains the following Nuget packages:

>   [**Bodoconsult.App**: basic features nearly every app needs like configuration, logging, APM, Toast messages, business transactions and client notifications to simplify client communication...](doc/Bodoconsult.App/README.md)

>   [**Bodoconsult.App.WinForms**: more basic features for WinForms based apps](doc/Bodoconsult.App.WinForms/README.md)

>   [**Bodoconsult.App.Wpf**: more basic features for WPF based apps](doc/Bodoconsult.App.Wpf/README.md)

>   [**Bodoconsult.App.Wpf.Documents**: creating reports with WPF based apps](doc/Bodoconsult.App.Wpf.Documents/README.md)

>   [**Bodoconsult.App.Avalonia**: more basic features for Avalonia based apps](doc/Bodoconsult.App.Avalonia/README.md)

>   [**Bodoconsult.Pdf**: simplified generation of PDF files](doc/Bodoconsult.Pdf/README.md)

>   [**Bodoconsult.Text**: generating structured text documents like reports with IDocumentBuilder and export them to HTML, RTF, DOCX, XPS and PDF](doc/Bodoconsult.Text/README.md)

>   [**Bodoconsult.Test**: generate a structured documentation of unit test results i.e. for auditors](doc/Bodoconsult.Test/README.md)

>   [**Bodoconsult.App.BackgroundService**: more basic features for based background service apps hosting no GRPC service](doc/Bodoconsult.App.BackgroundService/README.md)

>   [**Bodoconsult.App.GrpcBackgroundService**: more basic features for based background service apps hosting a GRPC service](doc/Bodoconsult.App.GrpcBackgroundService/README.md)

>   [**Bodoconsult.App.Windows**: MS Windows specific tools](doc/Bodoconsult.App.Windows/README.md)

>   [**Bodoconsult.I18N**: standardized internationalisation for apps based on service, console, WinFirms, WPF or Avalonia](doc/Bodoconsult.I18N/README.md)

>   [**Bodoconsult.Drawing**: System.Drawing based graphic functionality](doc/Bodoconsult.Drawing/README.md)

>   [**Bodoconsult.Drawing.SkiaSharp**: SkiaSharp based graphic functionality](doc/Bodoconsult.Drawing.SkiaSharp/README.md)

>   [**Bodoconsult.Charting.Base**: Base definitions for charting](doc/Bodoconsult.Charting.Base/README.md)

>   [**Bodoconsult.Charting**: Charting based on ScottPlot and SkiaSharp](doc/Bodoconsult.Charting/README.md)

>   [**Bodoconsult.Office**: Exporting System.Data.DataTable to Excel file](doc/Bodoconsult.Office/README.md) 

The following Nuget packages are deprecated starting with 1.0.8

>   **Bodoconsult.Text.Pdf**: this package was integrated into Bodoconsult.Text

# Features missing currently

-   Generating structured text documents like reports with IDocumentBuilder: DOCX export to implement

-   Generating structured text documents like reports with IDocumentBuilder: multi-column documents and chart-with-data-blocks


# About us

Bodoconsult (<http://www.bodoconsult.de>) is a Munich based software development company from Germany.

Robert Leisner is senior software developer at Bodoconsult. See his profile on <http://www.bodoconsult.de/Curriculum_vitae_Robert_Leisner.pdf>.