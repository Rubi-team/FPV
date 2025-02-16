using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using Utils;

public class UnityServiceAuthentification : BaseInstance<UnityServiceAuthentification>
{
    public event EventHandler<EventArgs> OnAuthentificateSuccess;


    public async Task<Task> Initialise()
    {
        try
        {
            await UnityServices.InitializeAsync();

            if (AuthenticationService.Instance.IsSignedIn) // Prevent double sign-in
                Debug.LogWarning("Already signed in. Skipping authentication.");
            else
                await AuthenticationService.Instance.SignInAnonymouslyAsync();

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
            };

            OnAuthentificateSuccess?.Invoke(this, EventArgs.Empty);
        }
        catch (Exception e)
        {
            Debug.LogError("An error occurred during authentication: " + e.Message);
            Initialise(); // Retry authentication
        }

        return Task.CompletedTask;
    }
}