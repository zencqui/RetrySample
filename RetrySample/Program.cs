using RestSharp;
using RetrySample;

HttpClientHandler httpClientHandler = new HttpClientHandler();
TokenRefreshHandler tokenRefreshHandler = new TokenRefreshHandler(httpClientHandler);

using (HttpClient client = new HttpClient(tokenRefreshHandler))
{
    client.DefaultRequestHeaders.Add("Authorization", "Bearer b29f0433ccdfec3222152fb3fce4f972");

    var options = new RestClientOptions("https://example.com/api/")
    {
        ConfigureMessageHandler = handler => tokenRefreshHandler
    };
    var restClient = new RestClient(options);

    HttpResponseMessage response = await client.GetAsync("/endpoint");
    Console.WriteLine(response.StatusCode);
}