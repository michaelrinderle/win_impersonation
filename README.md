# win_impersonator

C# wrapper for Win32 Impersonation API
 
- Perfect for running limited escalated Unit Tests. 

# interface

```csharp

// user the current user to impersonate

using (var wi = new WinImpersonation())
{
    wi.RunImpersonatedCode(() =>
    {
        var user = WindowsIdentity.GetCurrent().Name
    });
}

// target a specific user to impersonate

using (var wi = new WinImpersonation("username", "domain", "password"))
{
    wi.RunImpersonatedCode(() =>
    {
        var user = WindowsIdentity.GetCurrent().Name
    });
}

```