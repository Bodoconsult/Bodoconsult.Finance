// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH.  All rights reserved.

namespace BodoMoneyTransfer.Dal.Delegates;

/// <summary>
/// Delegate for sending an error mail to the admin
/// </summary>
/// <param name="subject">Subject of the mail</param>
/// <param name="body">Mail body</param>
public delegate void SendMailDelegate(string subject, string body);

