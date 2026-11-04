using Bodoconsult.App;
using Bodoconsult.App.Abstractions.Delegates;
using Bodoconsult.App.Abstractions.Interfaces;
using FinanzDbUpdater.Bll.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;

namespace FinanzDbUpdater.Bll.App;

/// <summary>
/// App global values
/// </summary>
public class Globals : IFinanzDbUpdaterAppGlobals
{
    #region Singleton factory

    // Thread-safe implementation of singleton pattern
    private static Lazy<Globals> _instance;

    /// <summary>
    /// Get a singleton instance of 
    /// </summary>
    /// <returns></returns>
    public static Globals Instance
    {
        get
        {
            try
            {
                _instance ??= new Lazy<Globals>(() => new Globals());
                return _instance.Value;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    #endregion

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
    public void Dispose()
    {
        //throw new NotImplementedException();
    }

    /// <summary>
    /// This event is set if the application is started only as singleton
    /// </summary>
    public EventWaitHandle EventWaitHandle { get; set; }

    /// <summary>
    /// App start parameter
    /// </summary>
    public IAppStartParameter AppStartParameter { get; set; } = new AppStartParameter();

    /// <summary>
    /// Current log data entry factory
    /// </summary>
    public ILogDataFactory LogDataFactory { get; set; } = new LogDataFactory();

    /// <summary>
    /// Current logging config
    /// </summary>
    public LoggingConfig LoggingConfig { get; set; } = new();

    /// <summary>
    /// Current app logger. Use this instance only if no DI container is available. Nonetheless, use DiContainer.Get&lt;IAppLoggerProxy&gt; to fetch the default app logger from DI container. Don't forget to load it during DI setup!
    /// </summary>
    public IAppLoggerProxy Logger { get; set; }

    /// <summary>
    /// Current dependency injection (DI) container
    /// </summary>
    public Bodoconsult.App.Abstractions.DependencyInjection.DiContainer DiContainer { get; set; } = new();

    /// <summary>
    /// Delegate called if a fatale app exception has been raised and a message to the UI has to be sent before app terminates
    /// </summary>
    public HandleFatalExceptionDelegate HandleFatalExceptionDelegate { get; set; }

    /// <summary>
    /// Current app storage connection check instance or null
    /// </summary>
    public IAppStorageConnectionCheck AppStorageConnectionCheck { get; set; }

    /// <summary>
    /// Current status message delegate
    /// </summary>
    public StatusMessageDelegate StatusMessageDelegate { get; set; }

    /// <summary>
    /// Current license management delegate
    /// </summary>
    public LicenseMissingDelegate LicenseMissingDelegate { get; set; }

    /// <summary>
    /// Delegate to handle I18N translations
    /// </summary>
    public TranslateDelegate TranslateDelegate { get; set; }

    /// <summary>
    /// Delegate to handle I18N translations with parameters to fill in translated text
    /// </summary>
    public TranslateWithParamsDelegate TranslateWithParamsDelegate { get; set; }

    /// <summary>
    /// Externally registered product name
    /// </summary>
    public string ProductName { get; set; }

    /// <summary>
    /// Externally registered version
    /// </summary>
    public string ProductVersion { get; set; }

    /// <summary>
    /// The current configuration loaded from appsettings.json
    /// </summary>
    public IConfigurationRoot ConfigurationRoot { get; set; }

    /// <summary>
    /// Path to archive
    /// </summary>
    public string ArchivePath { get; set; }

    /// <summary>
    /// Full path to the Depot data file
    /// </summary>
    public string DepotJsonFile { get; set; }

    /// <summary>
    /// Full path to the logo for the simulation report
    /// </summary>
    public string LogoSimulationReport { get; set; }

    /// <summary>
    /// Full path to the file with the quotation data from the bank
    /// </summary>
    public string QuotationJsonFile { get; set; }

    /// <summary>
    /// Current delegate to handle exceptions
    /// </summary>
    public HandleExceptionDelegate HandleExceptionDelegate { get; set; }
}