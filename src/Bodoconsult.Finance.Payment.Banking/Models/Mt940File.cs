//// Copyright (c) Bodoconsult EDV-Dienstleistungen GmbH. All rights reserved.

//using System;
//using System.Collections.Generic;
//using Bodoconsult.Finance.Payment.Base.Models;

//namespace Bodoconsult.Finance.Payment.Banking.Models;

//public class Mt940File
//{

//    public string References { get; set; }

//    public string AccountOwner { get; set; }

//    public string AccountNumber { get; set; }

//    public string StatementNumber { get; set; }

//    public decimal OpeningBalanceAmount { get; set; }

//    public DateTime OpeningBalanceDate { get; set; }

//    public string OpeningBalanceCurrency { get; set; }

//    public decimal ClosingBalanceAmount { get; set; }

//    public DateTime ClosingBalanceDate { get; set; }

//    public string ClosingBalanceCurrency { get; set; }

//    public decimal AvailableBalanceAmount { get; set; }

//    public DateTime AvailableBalanceDate { get; set; }

//    public string AvailableBalanceCurrency { get; set; }

//    public IList<AccountStatementLine> Items { get; set; } 

//    public Mt940File()
//    {
//        References = "STARTUMS";
//        StatementNumber = "0";
//        OpeningBalanceCurrency = "EUR";
//        ClosingBalanceCurrency = "EUR";
//        AvailableBalanceCurrency = "EUR";
//    }
//}