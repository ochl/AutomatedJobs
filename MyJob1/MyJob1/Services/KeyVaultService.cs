using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace MyJob1.Services
{
    public class KeyVaultService
    {
        private readonly SecretClient _client;

        public KeyVaultService()
        {
            _client = new SecretClient(
                new Uri("https://ochitserviceskeyvault.vault.azure.net/"),
                new DefaultAzureCredential(new DefaultAzureCredentialOptions
                {
                    AdditionallyAllowedTenants = { "*" }
                }));
        }

        public async Task<string> GetSecretAsync(string name)
        {
            var secret = await _client.GetSecretAsync(name);
            return secret.Value.Value;
        }
    }
}