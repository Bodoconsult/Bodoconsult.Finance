// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace BodoMoneyTransfer.Dal.Interfaces;

/// <summary>
/// Factory interface for <see cref="IWorkloadProcessor"/> objects
/// </summary>
public interface IWorkloadProcessorFactory
{
    /// <summary>
    /// Get a <see cref="IWorkloadProcessor"/>
    /// </summary>
    /// <returns><see cref="IWorkloadProcessor"/> instance</returns>
    IWorkloadProcessor CreateInstance();
}