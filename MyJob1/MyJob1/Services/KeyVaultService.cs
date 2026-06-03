using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

public class KeyVaultService
{
    private readonly SecretClient _client;

    public KeyVaultService()
    {
        _client = new SecretClient(
            new Uri("https://ochitserviceskeyvault.vault.azure.net/"),
            new DefaultAzureCredential());
    }

    public async Task<string> GetSecretAsync(string name)
    {
        var secret = await _client.GetSecretAsync(name);
        return secret.Value.Value;
    }
}