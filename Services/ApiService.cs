using System.Diagnostics;
using System.Net.Http.Json;
using MyShopApi.Delegates;

namespace MyShopApi.Services;

public class ApiService
{
    private readonly HttpClient _client;

    public event OnRequestCompleted? RequestCompleted;

    public ApiService(HttpClient client)
    {
        _client = client;
    }

    // GET список или объект
    public async Task GetAsync(string url)
    {
        var sw = Stopwatch.StartNew();

        var response = await _client.GetAsync(url);

        sw.Stop();

        RequestCompleted?.Invoke(url, (int)response.StatusCode, sw.ElapsedMilliseconds);
    }

    // POST создание
    public async Task PostAsync<T>(string url, T data)
    {
        var sw = Stopwatch.StartNew();

        var response = await _client.PostAsJsonAsync(url, data);

        sw.Stop();

        RequestCompleted?.Invoke(url, (int)response.StatusCode, sw.ElapsedMilliseconds);
    }

    // PUT обновление
    public async Task PutAsync<T>(string url, T data)
    {
        var sw = Stopwatch.StartNew();

        var response = await _client.PutAsJsonAsync(url, data);

        sw.Stop();

        RequestCompleted?.Invoke(url, (int)response.StatusCode, sw.ElapsedMilliseconds);
    }

    // DELETE удаление
    public async Task DeleteAsync(string url)
    {
        var sw = Stopwatch.StartNew();

        var response = await _client.DeleteAsync(url);

        sw.Stop();

        RequestCompleted?.Invoke(url, (int)response.StatusCode, sw.ElapsedMilliseconds);
    }
}