//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using System;

//namespace Bodoconsult.Finance.Payment.Banking.Dta;

//public interface IPaymentExchangeFile: IDisposable
//{
//    DtaMode TransactionMode { get; set; }
//    string ReceiverBankCode { set; }
//    string ReceiverAccount { set; }
//    string ReceiverName { set; }
//    DateTime TransferDate { set; }
//    string Standardtext { set; }
//    string[] TableNames { get; }
//    string TableName { set; }

//    /// <summary>
//    /// Vollständiger Pfad zur Excel-Datei
//    /// </summary>
//    string FileName { get; set; }

//    string TargetDtaFile { set; }
//    DtaStructure Structure { get; set; }
//    void ReadSource();

//    /// <summary>
//    /// Lastschriftdatei erzeugen
//    /// </summary>
//    void CreateFile();

//    void Quit();

//}