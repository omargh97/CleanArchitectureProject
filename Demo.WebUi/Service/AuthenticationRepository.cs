using Blazored.LocalStorage;
using Demo.ViewModel;
using Demo.WebUi.IService;
using Demo.WebUi.Providers;
using Demo.WebUi.Static;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Demo.WebUi.Service
{
    public class AuthenticationRepository : IAuthenticationRepository
    {

        private readonly IHttpClientFactory _client;
        private readonly ILocalStorageService _localStorage;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public AuthenticationRepository(IHttpClientFactory client, ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider)
        {
            _client = client;
            _localStorage = localStorage;
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<bool> Login(LoginRequestVM loginRequest)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, EndPoints.LoginEndPoints);
            if (loginRequest == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json");

            HttpClient client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return false;
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                LoginResultVM? responsejson = JsonConvert.DeserializeObject<LoginResultVM>(content);

                await _localStorage.SetItemAsync("authToken", responsejson?.AccessToken);
                await _localStorage.SetItemAsync("authUserName", responsejson?.UserName);
                await _localStorage.SetItemAsync("authOriginalUserName", responsejson?.OriginalUserName);

                await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", responsejson?.AccessToken);
                return true;
            }
        }

        public async Task Logout()
        {
            await _localStorage.ClearAsync();
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
        }

        public async Task<bool> Signin(SigninVM signin)
        {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, EndPoints.SigninEndPoints);
            if (signin == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(signin), Encoding.UTF8, "application/json");

            HttpClient client = _client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            return response.IsSuccessStatusCode;

        }
    }
}
