using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;
using Newtonsoft.Json;

public class CloudSaveManager : MonoBehaviour
{
    private async void Awake()
    {
        if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
        {
            await UnityServices.InitializeAsync();
        }

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Player authenticated successfully.");
        }
    }

    public async Task SaveUserData(string userID, string userPassword, string nickname)
    {
        try
        {
            await EnsureSignedIn();

            var existingData = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>());
            var dataToSave = new Dictionary<string, object>();

            // Preserve existing data
            foreach (var kvp in existingData)
            {
                dataToSave[kvp.Key] = kvp.Value;
            }

            string newKey = $"User_{Guid.NewGuid()}";
            var userData = new Dictionary<string, string>
            {
                { "UserID", userID },
                { "UserPassword", userPassword },
                { "Nickname", nickname }
            };

            dataToSave[newKey] = JsonConvert.SerializeObject(userData);

            await CloudSaveService.Instance.Data.ForceSaveAsync(dataToSave);
            Debug.Log("User data saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error saving user data: {ex.Message}");
        }
    }

    public async Task<List<Dictionary<string, string>>> LoadAllUserData()
    {
        try
        {
            await EnsureSignedIn();
            var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string>());

            var userList = new List<Dictionary<string, string>>();

            foreach (var kvp in data)
            {
                try
                {
                    var userData = JsonConvert.DeserializeObject<Dictionary<string, string>>(kvp.Value);
                    userList.Add(userData);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning($"Failed to parse user data for key {kvp.Key}: {ex.Message}");
                }
            }

            return userList;
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error loading user data: {ex.Message}");
            return null;
        }
    }

    private async Task EnsureSignedIn()
    {
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("Player authenticated successfully.");
        }
    }

    //[ADDED] Helper to find user by ID and return the Nickname if found
    public async Task<string> GetNicknameByUserID(string userID)
    {
        var allUsers = await LoadAllUserData();
        if (allUsers == null) return null;

        var found = allUsers.FirstOrDefault(u => u["UserID"] == userID);
        if (found != null && found.ContainsKey("Nickname"))
        {
            return found["Nickname"];
        }
        return null;
    }
}
