using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class Program
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async Task Main()
    {
        try
        {
            string url = "https://api.coindesk.com/v1/bpi/currentprice.json";
            HttpResponseMessage response = await httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();


            var jsonObject = JsonDocument.Parse(responseBody);
            var bpiObject = jsonObject.RootElement.GetProperty("bpi");

            Console.WriteLine("╔═════════════════════════════════════════");
            Console.WriteLine("║Precios disponibles:");
            Console.WriteLine("╠═════════════════════════════════════════");
            foreach (var currency in bpiObject.EnumerateObject())
            {
                var code = currency.Name;
                var rate = currency.Value.GetProperty("rate").GetString();
                var description = currency.Value.GetProperty("description").GetString();

                Console.WriteLine($"║{code}: {rate} ({description})");
            }
            Console.WriteLine("╚═════════════════════════════════════════");
            Console.WriteLine();


            Console.Write("\nIngrese el código de la moneda para ver sus características: ");
            string? selectedCurrencyCode = Console.ReadLine();


            if (bpiObject.TryGetProperty(selectedCurrencyCode, out var selectedCurrency))
            {
                var symbol = selectedCurrency.GetProperty("symbol").GetString();
                var rate = selectedCurrency.GetProperty("rate").GetString();
                var description = selectedCurrency.GetProperty("description").GetString();
                var rateFloat = selectedCurrency.GetProperty("rate_float").GetDecimal();


                Console.WriteLine();
                Console.WriteLine( "╔═════════════════════════════════════╗");
                Console.WriteLine($"║Características del precio en {selectedCurrencyCode}:   ║");
                Console.WriteLine( "╠═════════════════════════════════════╣");
                Console.WriteLine($"║Símbolo: {symbol}");
                Console.WriteLine($"║Tasa: {rate}");
                Console.WriteLine($"║Descripción: {description}");
                Console.WriteLine($"║Tasa en Float: {rateFloat}");
                Console.WriteLine( "╚═════════════════════════════════════╝");
            }
            else
            {
                Console.WriteLine("\n¡Moneda no encontrada!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ocurrió un error: {ex.Message}");
        }
        finally
        {
            httpClient.Dispose();
        }
    }
}
