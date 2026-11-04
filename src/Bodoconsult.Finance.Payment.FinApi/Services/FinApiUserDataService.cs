using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Bodoconsult.Finance.Payment.FinApi.Api;
using Bodoconsult.Finance.Payment.FinApi.Client;
using Bodoconsult.Finance.Payment.FinApi.Model;

namespace Bodoconsult.Finance.Payment.FinApi.Services
{
    public class FinApiUserDataService
    {


        private readonly ClientConfigurationApi _configApi;

        private readonly BankConnectionsApi _connectionApi;

        private readonly string _url;
        private readonly string _clientId;
        private readonly string _clientSecret;
        private readonly string _userName;
        private readonly string _Password;

        public BankConnectionList BankConnections { get; set; }

        public List<Account> Accounts { get; set; }


        public FinApiUserDataService(string url, string clientId, string clientSecret, string userName, string password)
        {
            Accounts = new List<Account>();

            _url = url;
            _clientId = clientId;
            _clientSecret = clientSecret;
            _Password = password;
            _userName = userName;

            // Login
            _configApi = new ClientConfigurationApi(_url);

            var authApi = new AuthorizationApi(_configApi.Configuration);

            var token = authApi.GetToken("password", _clientId, _clientSecret, null, _userName, _Password);

            _configApi.Configuration.AccessToken = token._AccessToken;

            //// User service
            //usersApi = new UsersApi(configApi.Configuration);

            _connectionApi = new BankConnectionsApi(_configApi.Configuration);


        }













        public IList<DailyBalance> GetBalancesByDate(string bankCode, string accountNr, DateTime dateFrom, DateTime dateUntil)
        {

            var result = new List<DailyBalance>();

            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found : {bankCode}");
            }

            var bank = BankConnections.Connections.FirstOrDefault(x => x.Bank.Blz == bankCode);

            if (bank == null)
            {
                throw new Exception($"Bank not found : {bankCode}");
            }

            var bankId = bank.Id;

            var account = Accounts.FirstOrDefault(x => x.AccountNumber == accountNr && x.BankConnectionId == bankId);

            if (account == null)
            {
                throw new Exception($"Account not found : {accountNr}");
            }

            var accList = new List<long?> { account.Id };

            var accountApi = new AccountsApi(_configApi.Configuration);

            var sDateFrom = FormatDate(dateFrom);
            var sDateUntil = FormatDate(dateUntil);


            var results = accountApi.GetDailyBalances(accList, sDateFrom, sDateUntil);
            var pages = Convert.ToInt32(results.Paging.PageCount);
            var page = 1;

            while (true)
            {
                foreach (var item in results.DailyBalances)
                {
                    result.Add(item);
                }

                page++;
                if (page > pages) break;

                results = accountApi.GetDailyBalances(accList, sDateFrom, sDateUntil, null, page);
            }

            Debug.Print(results.Paging.TotalCount.ToString());
            Debug.Print(result.Count.ToString());


            return result;
        }


        public bool UpdateBankConnections()
        {
            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found!");
            }

//            // Migrate to X2SA
//            foreach (var bank in BankConnections.Connections)
//            {
//                MigrateBankConnectionToX2sa(bank);

//#if DEBUG
//                break;
//#endif
//            }


            foreach (var bank in BankConnections.Connections)
            {
                UpdateBankConnection(bank);
#if DEBUG
                break;
#endif
            }

            return true;
        }

        private void MigrateBankConnectionToX2sa(BankConnection bank)
        {
            var requestData = new ConnectInterfaceParams(bank.Id, ConnectInterfaceParams.InterfaceEnum.XS2A);

            try
            {
                var request = _connectionApi.ConnectInterface(requestData);
            }
            catch (ApiException e)
            {
                if (e.ErrorCode == 451)
                {
                    var conn = HandleWebformRequest(e);




                }
            }

            

        }


        private BankConnection HandleWebformRequest(ApiException e)
        {
            var errorData = (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(e.ErrorContent.ToString(), typeof(ErrorMessage));

            var webformId = Convert.ToInt64(errorData.Errors[0].Message);  // long? | Identifier of web form

            Process.Start(e.Url);

            var apiInstance = new WebFormsApi(_configApi.Configuration);


            while (true)
            {
                //try
                //{
                //Get a web form
                var result = apiInstance.GetWebForm(webformId);

                var status = result.Status;

                switch (status)
                {
                    case WebForm.StatusEnum.COMPLETED:

                        var conn = (BankConnection)_configApi.Configuration.ApiClient.DeserializeFromString(
                            result.ServiceResponseBody, typeof(BankConnection));

                        return conn;
                    case WebForm.StatusEnum.ABORTED:
                        return null;
                    case WebForm.StatusEnum.NOTYETOPENED:
                        //return null;
                        break;
                }

                Debug.WriteLine(result);
                //}
                //catch (Exception ex)
                //{
                //    Debug.Print("Exception when calling WebFormsApi.GetWebForm: " + ex.Message);
                //}
            }
        }


        public BankConnection UpdateBankConnection(BankConnection bank)
        {



            var requestData = new UpdateBankConnectionParams(bank.Id);

            // Step 1
            try
            {
                var request = _connectionApi.UpdateBankConnection(requestData);
                return request;
            }
            catch (ApiException e1)
            {
                UpdateStep1(requestData, e1, bank.Id);
            }

            try
            {
                var request = _connectionApi.UpdateBankConnection(requestData);
                return request;
            }
            catch (ApiException e2)
            {
                UpdateStep2(requestData, e2, bank.Id);
            }

            return null;

        }


        public bool UpdateBankConnectionsFromUi()
        {
            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found!");
            }

            foreach (var bank in BankConnections.Connections)
            {
                UpdateBankConnectionFromUi(bank);

            }

            return true;
        }

        public BankConnection UpdateBankConnectionFromUi(BankConnection bank)
        {

            var requestData = new UpdateBankConnectionParams(bank.Id);

            // Step 1
            try
            {
                var request = _connectionApi.UpdateBankConnection(requestData);
                return request;
            }
            catch (ApiException e1)
            {
                UpdateStep1(requestData, e1, bank.Id);
            }

            try
            {
                var request = _connectionApi.UpdateBankConnection(requestData);
                return request;
            }
            catch (ApiException e2)
            {
                UpdateStep2(requestData, e2, bank.Id);
            }



            //if (e.ErrorCode == 510)
            //{
            //    var errorData = (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(e.ErrorContent.ToString(), typeof(ErrorMessage));


            //}


            //if (e.ErrorCode == 451)
            //{
            //    var errorData = (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(e.ErrorContent.ToString(), typeof(ErrorMessage));

            //    var webformId = Convert.ToInt64(errorData.Errors[0].Message);  // long? | Identifier of web form

            //    Process.Start(e.Url);

            //    var apiInstance = new WebFormsApi(_configApi.Configuration);



            //    var status = WebForm.StatusEnum.INPROGRESS;

            //    while (true)
            //    {
            //        //try
            //        //{
            //        //Get a web form
            //        WebForm result = apiInstance.GetWebForm(webformId);

            //        status = result.Status;

            //        switch (status)
            //        {
            //            case WebForm.StatusEnum.COMPLETED:

            //                var conn = (BankConnection)_configApi.Configuration.ApiClient.DeserializeFromString(
            //                    result.ServiceResponseBody, typeof(BankConnection));

            //                return conn;
            //            case WebForm.StatusEnum.ABORTED:
            //                return null;
            //            case WebForm.StatusEnum.NOTYETOPENED:
            //                //return null;
            //                break;
            //        }

            //        Debug.WriteLine(result);
            //        //}
            //        //catch (Exception ex)
            //        //{
            //        //    Debug.Print("Exception when calling WebFormsApi.GetWebForm: " + ex.Message);
            //        //}
            //    }
            //}


            //catch (Exception e)
            //{
            //    throw new Exception("Other, non FinAPI related exception", e);
            //}


            return null;

        }

        private void UpdateStep2(UpdateBankConnectionParams requestData, ApiException e2, long? bankId)
        {
            if (e2.ErrorCode != 510)
            {
                throw new NotImplementedException();
            }

            var errorData =
                (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(
                    e2.ErrorContent.ToString(), typeof(ErrorMessage));


            Debug.Print($"{errorData.Errors.Count}");

        }

        private void UpdateStep1(UpdateBankConnectionParams requestData, ApiException e1,
            long? bankId)
        {
            if (e1.ErrorCode != 510)
            {
                throw new NotImplementedException();
            }

            var errorData =
                (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(
                    e1.ErrorContent.ToString(), typeof(ErrorMessage));



            var ed = (ErrorDetails)errorData.Errors[0];


            var mfa = ed.MultiStepAuthentication;

            var procId = mfa.TwoStepProcedures[0].ProcedureId;

            var callBack = new MultiStepAuthenticationCallback(mfa.Hash, null, procId);


            requestData.MultiStepAuthentication = callBack;

        }


        //public void GetUserData()
        //{

        //    var data = Task.Run(async() => await usersApi.GetAuthorizedUserAsync()).Result;

        //    Debug.Print(data.ToJson());
        //}


        /// <summary>
        /// Add a bank connection
        /// </summary>
        /// <param name="bankData"></param>
        public BankConnection AddBankConnection(BankData bankData)
        {

            // Bank service
            var userBanksApi = new BanksApi(_configApi.Configuration);
            var banks = userBanksApi.GetAndSearchAllBanks(null, bankData.Blz);

            // Get all bank connection
            var connectionList = _connectionApi.GetAllBankConnections();


            // Check if bank already exists
            var bank = banks.Banks.FirstOrDefault(x => x.Blz == bankData.Blz);

            if (bank == null)
            {
                throw new Exception($"Bank with BLZ {bankData.Blz} does not exists!");
            }

            var id = bank.Id;
            if (connectionList.Connections.Any(x => x.BankId == id))
            {
                throw new Exception($"Bank with BLZ {bankData.Blz} already exists!");
            };

            var data = new ImportBankConnectionParams(id, bankData.BankUser, null, null, bankData.BankPin, true);

            BankConnection connection = null;
            //try
            //{
            connection = _connectionApi.ImportBankConnection(data);
            //}
            //catch (ApiException e)
            //{


            //    if (e.ErrorCode == 451)
            //    {
            //        var errorData = (ErrorMessage)_configApi.Configuration.ApiClient.DeserializeFromString(e.ErrorContent.ToString(), typeof(ErrorMessage));

            //        var webformId = Convert.ToInt64(errorData.Errors[0].Message);  // long? | Identifier of web form

            //        Process.Start(e.Url);

            //        var apiInstance = new WebFormsApi(_configApi.Configuration);

            //        var status = WebForm.StatusEnum.INPROGRESS;

            //        while (status == WebForm.StatusEnum.INPROGRESS)
            //        {
            //            try
            //            {
            //                //Get a web form
            //                WebForm result = apiInstance.GetWebForm(webformId);

            //                status = result.Status;

            //                switch (status)
            //                {
            //                    case WebForm.StatusEnum.COMPLETED:
            //                        return (BankConnection)_configApi.Configuration.ApiClient.DeserializeFromString(
            //                            result.ServiceResponseBody, typeof(BankConnection));
            //                    case WebForm.StatusEnum.ABORTED:
            //                    case WebForm.StatusEnum.NOTYETOPENED:
            //                        return null;
            //                }

            //                Debug.WriteLine(result);
            //            }
            //            catch (Exception ex)
            //            {
            //                Debug.Print("Exception when calling WebFormsApi.GetWebForm: " + ex.Message);
            //            }
            //        }
            //    }

            //}
            //catch (Exception e)
            //{
            //    throw new Exception("Other, non FinAPI related exception", e);
            //}

            return connection;

        }

        public int GetBankConnections(bool updateConnections = true)
        {
            // Get all bank connections
            BankConnections = _connectionApi.GetAllBankConnections();

            if (updateConnections)
            {
                UpdateBankConnections();
            }

            return BankConnections.Connections.Count;
        }


        public int GetAccounts()
        {

            if (BankConnections == null)
            {
                return 0;
            }

            var accountApi = new AccountsApi(_configApi.Configuration);

            foreach (var conn in BankConnections.Connections)
            {
                var result = accountApi.GetAndSearchAllAccounts(conn.AccountIds);

                foreach (var account in result.Accounts)
                {
                    //account.BankConnectionId = conn.Id;
                    Accounts.Add(account);
                }
            }

            return Accounts.Count;
        }

        public IList<Transaction> GetTransactions(string bankCode, string accountNr, DateTime datefrom, DateTime dateUntil)
        {

            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found : {accountNr}");
            }

            var bank = BankConnections.Connections.FirstOrDefault(x => x.Bank.Blz == bankCode);

            if (bank == null)
            {
                throw new Exception($"Bank not found : {bankCode}");
            }

            var bankId = bank.Id;

            var account = Accounts.FirstOrDefault(x => x.AccountNumber == accountNr && x.BankConnectionId == bankId);

            if (account == null)
            {
                throw new Exception($"Account not found : {bankCode} // {accountNr} // Accounts: {Accounts.Count}");
            }

            var accList = new List<long?> { account.Id };


            var sDateFrom = FormatDate(datefrom);
            var sDateUntil = FormatDate(dateUntil);

            var result = new List<Transaction>();

            var api = new TransactionsApi(_configApi.Configuration);
            var results = api.GetAndSearchAllTransactions("bankView", null, null, null, null,
                accList, sDateFrom, sDateUntil);

            var pages = Convert.ToInt32(results.Paging.PageCount);
            var page = 1;

            while (true)
            {
                foreach (var item in results.Transactions)
                {
                    result.Add(item);
                }

                page++;
                if (page > pages) break;

                results = api.GetAndSearchAllTransactions("bankView", null, null, null, null,
                    accList, sDateFrom, sDateUntil, null, null, null, null, null, null,
                    null, null, null, null, null, null, null, page);
            }

            Debug.Print(results.Paging.TotalCount.ToString());
            Debug.Print(result.Count.ToString());


            return result;
        }


        public IList<Security> GetDepot(string bankCode, string accountNr)
        {

            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found : {accountNr}");
            }

            var bank = BankConnections.Connections.FirstOrDefault(x => x.Bank.Blz == bankCode);

            if (bank == null)
            {
                throw new Exception($"Bank not found : {bankCode}");
            }

            var bankId = bank.Id;

            var account = Accounts.FirstOrDefault(x => x.AccountNumber == accountNr && x.BankConnectionId == bankId);

            if (account == null)
            {
                throw new Exception($"Account not found : {bankCode} // {accountNr}");
            }


            var result = new List<Security>();
            var accList = new List<long?> { account.Id };

            var api = new SecuritiesApi(_configApi.Configuration);
            var results = api.GetAndSearchAllSecurities(null, null, accList);


            var pages = Convert.ToInt32(results.Paging.PageCount);
            var page = 1;

            while (true)
            {
                foreach (var item in results.Securities)
                {
                    result.Add(item);
                }

                page++;
                if (page > pages) break;

                results = api.GetAndSearchAllSecurities(null, null, accList, page);
            }

            Debug.Print(results.Paging.TotalCount.ToString());
            Debug.Print(result.Count.ToString());


            return result;
        }


        private static string FormatDate(DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public decimal GetBalance(string bankCode, string accountNr)
        {


            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found : {accountNr}");
            }

            var bank = BankConnections.Connections.FirstOrDefault(x => x.Bank.Blz.Trim() == bankCode.Trim());

            if (bank == null)
            {
                throw new Exception($"Bank not found : {bankCode}");
            }

            var bankId = bank.Id;

            //Account account=null;
            //for (var i = 0; i< Accounts.Count; i++)
            //{

            //    var item = Accounts[i];
            //    if (item.BankConnectionId == bankId)
            //    {
            //        if (item.AccountNumber == accountNr)
            //        {
            //            account = item;
            //            throw new Exception($"Account found");
            //        }

            //    }

            //}


            var account = Accounts.FirstOrDefault(x =>
                x.BankConnectionId == bankId && x.AccountNumber.Trim() == accountNr.Trim());
            //var account = GetAccount(bankCode, accountNr);

            //var account = Accounts.FirstOrDefault(x =>
            //    x.AccountNumber.Equals(accountNr.Trim())  && x.BankConnectionId == bankId);

            if (account == null)
            {
                throw new Exception($"Account not found : {bankCode} // {accountNr} // Bank: {bank.Id} // Accounts: {Accounts.Count} // JSON: " + JsonConvert.SerializeObject(Accounts, Formatting.Indented));
            }

            return account.Balance ?? 0;
        }

        //private Account GetAccount(string bankCode, string accountNr)
        //{


        //}


        public string GetOwner(string bankCode, string accountNr)
        {
            if (BankConnections == null)
            {
                throw new Exception($"No bank connections not found : {accountNr}");
            }



            var bank = BankConnections.Connections.FirstOrDefault(x => x.Bank.Blz == bankCode);

            if (bank == null)
            {
                throw new Exception($"Bank not found : {bankCode}");
            }

            var bankId = bank.Id;

            var account = Accounts.FirstOrDefault(x => x.AccountNumber == accountNr && x.BankConnectionId == bankId);

            if (account == null)
            {
                throw new Exception($"Account not found : {bankCode} // {accountNr}");
            }

            return account.AccountHolderName;
        }


        public void SetUserToAotomatcUpdate()
        {
            var usersApi = new UsersApi(_configApi.Configuration);

            var u = new UserUpdateParams { IsAutoUpdateEnabled = true };

            var response = usersApi.EditAuthorizedUser(u);
        }


    }
}
