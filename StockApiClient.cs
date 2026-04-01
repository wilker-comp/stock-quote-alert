using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
//aqui montei um frankstein e pedi ajuda da IA na escrita e compilar pois n estava indo direito, ja tinha feito integracao de api num projeto do ime antes mas muito diferente disso
namespace StockQuoteAlert
{
  public class StockApiClient
  {
    private readonly HttpClient _httpClient;
    private readonly string _token;
    private const string BaseUrl = "https://brapi.dev/api/quote";

    public StockApiClient(string token)
    {
      _httpClient = new HttpClient();
      _token = token;
    }

    public async Task<decimal> GetCurrentPriceAsync(string ticker)
    {
      try
      {
        // essa parte da url é de boa
        string url = $"{BaseUrl}/{ticker}?token={_token}";

        // dispara get, aqui n sabia direito, pedi ajuda na escrita
        HttpResponseMessage response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode(); // só p n ficar errado

        string jsonResponse = await response.Content.ReadAsStringAsync();

        // essa parte de navegar pelo json sabia zero, pedi ajuda
        using JsonDocument doc = JsonDocument.Parse(jsonResponse);
        JsonElement root = doc.RootElement;

        decimal price = root.GetProperty("results")[0].GetProperty("regularMarketPrice").GetDecimal();

        return price;
      }
      catch (Exception ex)
      {
        Console.WriteLine($"\n[ERRO API] Não foi possível obter o preço de {ticker}. Detalhe: {ex.Message}");
        throw;
      }
    }
  }
}