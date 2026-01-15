# End User ID Integration

This SDK now includes a persistent `end_user_id` that is sent with every `ConvaiRoomRequest`. By default, the value is derived from Unity's `SystemInfo.deviceUniqueIdentifier`, but you can plug in your own logic without touching the core code.

## Runtime behavior

- `ConvaiRoomManager` calls an `IEndUserIdProvider` implementation before requesting room details.
- `DeviceEndUserIdProvider` (the default) returns the stored `ConvaiConfigurationDataSO.EndUserId`, or falls back to `SystemInfo.deviceUniqueIdentifier` and finally a generated GUID.
- The resolved identifier is written back to `ConvaiConfigurationDataSO`, ensuring it persists between sessions.
- Key runtime files involved in this flow:
  - `Assets/Convai/Scripts/Runtime/Networking/Transport/ConvaiRoomManager.cs`
  - `Assets/Convai/Scripts/Runtime/Networking/Transport/EndUserIdProvider.cs`
  - `Assets/Convai/Scripts/Runtime/Player/ConvaiPlayer.cs`
  - `Assets/Convai/Scripts/Runtime/Configuration/ConvaiConfigurationDataSO.cs`
  - `Assets/Convai/Scripts/Runtime/Configuration/ConvaiConfigurationDataSystem.cs`
  - `Assets/Convai/Plugins/convai-csharp-rest-main/ConvaiRestModels.cs`

## Providing custom logic

1. Implement the `IEndUserIdProvider` interface in your own class:
   ```csharp
   using Convai.Scripts.Configuration;
   using Convai.Scripts.Networking.Transport;

   public sealed class AccountScopedEndUserIdProvider : IEndUserIdProvider
   {
       public string GetOrCreateEndUserId(ConvaiConfigurationDataSO config)
       {
           // Example: use your account system user id
           string accountId = MyAuthService.CurrentUserId;
           if (string.IsNullOrEmpty(accountId))
           {
               return new DeviceEndUserIdProvider().GetOrCreateEndUserId(config);
           }

           if (!string.Equals(config.EndUserId, accountId, StringComparison.Ordinal))
           {
               config.EndUserId = accountId;
               config.Save();
           }

           return accountId;
       }
   }
   ```

2. Assign the provider before the first room connection (for example, in a bootstrap MonoBehaviour):
   ```csharp
   void Awake()
   {
       if (ConvaiRoomManager.Instance != null)
       {
           ConvaiRoomManager.Instance.EndUserIdProvider = new AccountScopedEndUserIdProvider();
       }
   }
   ```

That’s it—the custom provider will run for every subsequent room request, ensuring all backend calls use your preferred end-user identifier.
