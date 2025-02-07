using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sync
{
    /// <summary>
    /// API Docs found at https://docs.virtuoussoftware.com/
    /// </summary>
    internal class VirtuousService
    {
        private readonly RestClient _restClient;

        public VirtuousService(IConfiguration configuration) 
        {
            var apiBaseUrl = configuration.GetValue("VirtuousApiBaseUrl");
            var apiKey = configuration.GetValue("VirtuousApiKey");

            var options = new RestClientOptions(apiBaseUrl)
            {
                Authenticator = new RestSharp.Authenticators.OAuth2.OAuth2AuthorizationRequestHeaderAuthenticator(apiKey)
            };

            _restClient = new RestClient(options);
        }

        public async Task<PagedResult<AbbreviatedContact>> GetContactsAsync(int skip, int take)
        {
            try
            {
                var request = new RestRequest("/api/Contact/Query/FullContact", Method.Post);
                request.AddQueryParameter("Skip", skip);
                request.AddQueryParameter("Take", take);

                var body = new ContactQueryRequest();
                request.AddJsonBody(body);

                var response = await _restClient.PostAsync<PagedResult<AbbreviatedContact>>(request);
                if (response == null || response.List == null)
                {
                    return new PagedResult<AbbreviatedContact> { List = new List<AbbreviatedContact>(), Total = 0 };
                }

                response.List = response.List
                                            .Where(c => c.Address != null && c.Address.State == "AZ")
                                            .ToList();
                return response;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error fetching contacts: {ex.Message}");
                return new PagedResult<AbbreviatedContact> { List = new List<AbbreviatedContact>(), Total = 0 };
            }
            
        }
    }
}
