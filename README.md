# win_impersonator

C# wrapper for Win32 Impersonation API
 
- Perfect for running limited escalated Unit Tests. 

# interface

```

// user the current user to impersonate

using (var wi = new WinImpersonation())
{
    wi.RunImpersonatedCode(() =>
    {
        var cipherText = RegistryServiceProvider.GetRegistryKeyValue();
        Assert.IsNotNull(cipherText);

        var json = JsonConvert.DeserializeObject<ApiCredentials>(
            Encoding.UTF8.GetString(cipherText));

        Assert.IsTrue((JsonConvert.SerializeObject(json) == JsonConvert.SerializeObject(ApiCredentials)));
    });
}

// target a specific user to impersonate

using (var wi = new WinImpersonation("username", "domain", "password"))
{
    wi.RunImpersonatedCode(() =>
    {
        var cipherText = RegistryServiceProvider.GetRegistryKeyValue();
        Assert.IsNotNull(cipherText);

        var json = JsonConvert.DeserializeObject<ApiCredentials>(
            Encoding.UTF8.GetString(cipherText));

        Assert.IsTrue((JsonConvert.SerializeObject(json) == JsonConvert.SerializeObject(ApiCredentials)));
    });
}

```