namespace BodoMoneyTransfer.Dal.Model;

/// <summary>
/// Describes a job task for collecting data from diverse relevante databases
/// </summary>
public class JobTask
{
    /// <summary>
    /// SQL to execute
    /// </summary>
    public string Sql { get; set; }
}