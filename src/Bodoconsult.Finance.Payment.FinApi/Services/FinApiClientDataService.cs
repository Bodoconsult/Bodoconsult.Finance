using Bodoconsult.Finance.Payment.FinApi.Api;
using Bodoconsult.Finance.Payment.FinApi.Model;

namespace Bodoconsult.Finance.Payment.FinApi.Services
{
    /// <summary>
    /// Handles client related tasks
    /// </summary>
    public class FinApiClientDataService
    {
        private ClientConfigurationApi configApi;
        private AuthorizationApi authApi;

        private BankConnectionsApi connectionApi;

        private string _url;
        private string _clientId;
        private string _clientSecret;

        private UsersApi usersApi;
        private BanksApi banksApi;



        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="url"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        public FinApiClientDataService(string url, string clientId, string clientSecret)
        {
            _url = url;
            _clientId = clientId;
            _clientSecret = clientSecret;

            // Login
            configApi = new ClientConfigurationApi(_url);

            authApi = new AuthorizationApi(configApi.Configuration);

            var token = authApi.GetToken("client_credentials", _clientId, _clientSecret);

            configApi.Configuration.AccessToken = token._AccessToken;

            // User service
            usersApi = new UsersApi(configApi.Configuration);

            

        }

        /// <summary>
        /// Add a user
        /// </summary>
        /// <param name="userName">Username</param>
        /// <param name="password">Password</param>
        /// <param name="email">Email address of the user</param>
        /// <param name="phone">Phone number of the user</param>
        /// <returns>true, if user was succesfully created</returns>
        public bool AddUser(string userName, string password, string email, string phone)
        {

            //
            var userData = new UserCreateParams
            {
                Email = email,
                Id = userName,
                Password = password,
                Phone = phone,
            };

            //try
            //{
                var response = usersApi.CreateUser(userData);
            //}
            //catch
            //{
            //    return false;
            //}

            return true;
        }


    }
}