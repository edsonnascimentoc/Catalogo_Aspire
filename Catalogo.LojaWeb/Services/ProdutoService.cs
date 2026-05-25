using Catalogo.Domain;
using System.Text.Json;

namespace Catalogo.LojaWeb.Services;

public class ProdutoService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ProdutoService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ProdutoService(HttpClient httpClient, ILogger<ProdutoService> logger)
    {
        _httpClient = httpClient;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true // Ignorar diferenciação entre maiúsculas/minúsculas
        };
        _logger = logger;
    }

    public async Task<List<Produto>> GetProdutos()
    {
        List<Produto>? Produtos = null;

        try
        {
            var response = await _httpClient.GetAsync("/api/Produtos");
            var responseText = await response.Content.ReadAsStringAsync();

            _logger.LogInformation($"Http status code: {response.StatusCode}");
            _logger.LogInformation($"Http response content: {responseText}");

            if (response.IsSuccessStatusCode)
            {
                Produtos = await response.Content.ReadFromJsonAsync<List<Produto>>(_jsonOptions);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error ao executar GetProdutos.");
        }
        return Produtos ?? new List<Produto>();
    }
}
