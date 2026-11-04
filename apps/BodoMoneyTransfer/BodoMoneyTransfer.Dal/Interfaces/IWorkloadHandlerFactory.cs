using BodoMoneyTransfer.Dal.Model;

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Factory interface for <see cref="IWorkloadHandler"/> objects
/// </summary>
public interface IWorkloadHandlerFactory
{
    /// <summary>
    /// Get a <see cref="IWorkloadHandler"/>
    /// </summary>
    /// <param name="workload">Current workload to handle</param>
    /// <returns></returns>
    IWorkloadHandler CreateInstance(WorkloadTask workload);
}