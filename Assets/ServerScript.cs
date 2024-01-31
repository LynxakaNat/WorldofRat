
using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Multiplay;
using Unity.Services.RemoteConfig;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ServerScript : MonoBehaviour
{
    private MultiplayEventCallbacks callbacks;
    private IServerQueryHandler m_ServerQueryHandler;
    private const ushort k_DefaultMaxPlayers = 10;
    private const string k_DefaultServerName = "MyServerExample";
    private const string k_DefaultGameType = "MyGameType";
    private const string k_DefaultBuildId = "MyBuildId";
    private const string k_DefaultMap = "MyMap";
    private float pollTicketTimer;
    private float pollTicketTimerMax = 1.1f;
    private CreateTicketResponse createTicketResponse;
    public Button startgame;
    public struct userAttributes { }
    public struct appAttributes { }
    public float maxpspeed;
    // Start is called before the first frame update
    async void Start()
    {
        InitializationOptions options = new InitializationOptions();
        await UnityServices.InitializeAsync(options);
        
#if !DEDICATED_SERVER
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
#endif
#if DEDICATED_SERVER
        var serverConfig = MultiplayService.Instance.ServerConfig;
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
        //NetworkManager.Singleton.StartServer();
        callbacks = new MultiplayEventCallbacks();
        callbacks.Allocate += OnAllocate;
        callbacks.Deallocate += OnDeallocate;
        callbacks.Error += OnError;
        callbacks.SubscriptionStateChanged += OnSubscriptionStateChanged;
        m_ServerQueryHandler = await MultiplayService.Instance.StartServerQueryHandlerAsync(k_DefaultMaxPlayers, k_DefaultServerName, k_DefaultGameType, k_DefaultBuildId, k_DefaultMap);
        if (serverConfig.AllocationId != "") {
            OnAllocate(new MultiplayAllocation("", serverConfig.ServerId, serverConfig.AllocationId));
        }
        RemoteConfigService.Instance.FetchCompleted += ApplyRemoteSettings;
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        
#endif
    }
    void ApplyRemoteSettings(ConfigResponse configResponse)
    {
        maxpspeed = RemoteConfigService.Instance.appConfig.GetFloat("Speed");
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());
    }
    public async void FindMatch()
    {
        Debug.Log("Button worked");
        var players = new List<Player>
        {
            new Player(AuthenticationService.Instance.PlayerId, new Dictionary<string,object>())
        };
        var options = new CreateTicketOptions(
            "BadBitchesOnly",
            new Dictionary<string, object>());
        createTicketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);  
        pollTicketTimer = pollTicketTimerMax;

    }
    private async void PollMatchmakerTicket()
    {
        
        MultiplayAssignment assignment = null;
        bool gotAssignment = false;
        // Poll ticket
        TicketStatusResponse ticketStatus= await MatchmakerService.Instance.GetTicketAsync(createTicketResponse.Id);
        
        //Convert to platform assignment data (IOneOf conversion)
        if (ticketStatus.Type == typeof(MultiplayAssignment))
        {
            assignment = ticketStatus.Value as MultiplayAssignment;
        }

        switch (assignment?.Status)
        {
            case MultiplayAssignment.StatusOptions.Found:
                Debug.Log("Found a match");
                gotAssignment = true;
                break;
            case MultiplayAssignment.StatusOptions.InProgress:
                //...
                Debug.Log("Finding a match");
                break;
            case MultiplayAssignment.StatusOptions.Failed:
                Debug.Log("Failed to start a server");
                gotAssignment = true;
                Debug.LogError("Failed to get ticket status. Error: " + assignment.Message);
                break;
            case MultiplayAssignment.StatusOptions.Timeout:
                gotAssignment = true;
                Debug.LogError("Failed to get ticket status. Ticket timed out.");
                break;
            default:
                throw new InvalidOperationException();
        }
        if (gotAssignment)
        {
            createTicketResponse = null;
            Debug.Log(assignment.Ip + ' ' + assignment.Port);
            string ipv4Address = assignment.Ip;
            ushort port = (ushort)assignment.Port;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address, port);
            NetworkManager.Singleton.StartClient();
            startgame.gameObject.SetActive(false);
        }
    }
    private void StartServer()
    {
        //NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        //NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
        //NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartServer();
    }
    private void NetworkManager_ConnectionApprovalCallback()
    {

    }
    private void NetworkManager_OnClientConnectedCallback()
    {

    }
    private void NetworkManager_Server_OnClientDisconnectCallback() { }
    private async void OnAllocate(MultiplayAllocation allocation)
    {
        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        var serverConfig = MultiplayService.Instance.ServerConfig;
        string ipv4Address = "0.0.0.0";
        ushort port = serverConfig.Port;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData( ipv4Address, port, "0.0.0.0");
        // if it breaks just await
        StartServer();
        await MultiplayService.Instance.ReadyServerForPlayersAsync();
        Debug.Log($"Server ID[{serverConfig.ServerId}]");
        Debug.Log($"AllocationID[{serverConfig.AllocationId}]");
        Debug.Log($"Port[{serverConfig.Port}]");
        Debug.Log($"QueryPort[{serverConfig.QueryPort}");
        Debug.Log($"LogDirectory[{serverConfig.ServerLogDirectory}]");
        //ReadyServerForPlayersAsync()
    }
    private void OnDeallocate(MultiplayDeallocation deallocation)
    {
        NetworkManager.Singleton.Shutdown();
        Environment.Exit(0);
    }
    private void OnError(MultiplayError error)
    {

    }
    private void OnSubscriptionStateChanged(MultiplayServerSubscriptionState state)
    {
        switch (state)
        {
            case MultiplayServerSubscriptionState.Unsubscribed: /* The Server Events subscription has been unsubscribed from. */ break;
            case MultiplayServerSubscriptionState.Synced: /* The Server Events subscription is up to date and active. */ break;
            case MultiplayServerSubscriptionState.Unsynced: /* The Server Events subscription has fallen out of sync, the subscription tries to automatically recover. */ break;
            case MultiplayServerSubscriptionState.Error: /* The Server Events subscription has fallen into an errored state and won't recover automatically. */ break;
            case MultiplayServerSubscriptionState.Subscribing: /* The Server Events subscription is trying to sync. */ break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (createTicketResponse != null)
        {
            pollTicketTimer -= Time.deltaTime;
            if (pollTicketTimer < 0) {  pollTicketTimer = pollTicketTimerMax;
                PollMatchmakerTicket();
            }
        };
#if DEDICATED_SERVER
        m_ServerQueryHandler.UpdateServerCheck();
#endif
    }
}
