using Demo.WebUi.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Demo.WebUi.Service
{
    public class BaseRepository<T> : IBaseRepository<T> where T : class
    {
        private readonly IHttpClientFactory _Client;

        public BaseRepository(IHttpClientFactory client)
        {
            _Client = client;
        }

        public async Task<bool> Create(string url, T obj)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url);
            if (obj == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(obj));

            HttpClient client = _Client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.Created))
                return true;

            return false;
        }

        public async Task<bool> Delete(string url, int id)
        {
            if (id < 0)
                return false;

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, url);

            HttpClient client = _Client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.NoContent))
                return true;

            return false;
        }

        public async Task<T?> Get(string url, int id)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url + id);

            HttpClient client = _Client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                string content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                    return null;

                return JsonConvert.DeserializeObject<T>(content);
            }

            return null;
        }

        public async Task<IList<T>?> Get(string url)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);

            HttpClient client = _Client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.OK))
            {
                string content = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(content))
                    return null;

                return JsonConvert.DeserializeObject<IList<T>>(content);
            }

            return null;
        }

        public async Task<bool> Update(string url, T obj)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, url);
            if (obj == null)
                return false;

            request.Content = new StringContent(JsonConvert.SerializeObject(obj));

            HttpClient client = _Client.CreateClient();
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode.Equals(HttpStatusCode.NoContent))
                return true;

            return false;
        }
    }
}
