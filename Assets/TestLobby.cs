using Mono.CSharp;
using QFSW.QC;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class TestLobby : MonoBehaviour
{
    // Start is called before the first frame update

    private Lobby hostLobby;
    private float heartbeatTimer;
    private async void Start()              
    {
    
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeratbeatAsync();
    }


    private async Task HandleLobbyHeratbeatAsync()
    {
        if(hostLobby!=null)
        {
            heartbeatTimer = Time.deltaTime;
        }
        if(heartbeatTimer>0f)
        {
            float heartbeatTimerMax = 15;
            heartbeatTimer = heartbeatTimerMax;

            await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
        }
    }

    [Command]
    private async void CreateLobby() 
    {
        try
        {
            string lobbyName = "MyLobby";
            int maxPlayers = 4;

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);

            hostLobby = lobby;

            Debug.Log("created lobby!" + lobby.Name);
        }
        catch(LobbyServiceException e)
        {
            Debug.Log(e);
        }
       
    }

    [Command]
    private async void ListLobbies()
    {
        try
        {

            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions { 
                Count =25,
            
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots,"0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            Debug.Log("lobbies found: " + queryResponse.Results.Count);

            foreach (var result in queryResponse.Results)
            {
                Debug.Log(result.Name + " " + result.MaxPlayers);
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }


    [Command]
    private async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

}
