using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using System.Threading.Tasks;
using Unity.Netcode;
using System.Linq;
using NUnit.Framework;

public class Leaderboard : NetworkBehaviour
{
    
    public TMP_InputField nickname;
    public TMP_Text lead;
    const string LeaderboardId = "SpringBoss";
    // Start is called before the first frame update
    int Offset { get; set; }
    int Limit { get; set; } 
    public void Start()
    {
        //Adds a listener to the main input field and invokes a method when the value changes.
        nickname.onSubmit.AddListener(delegate { ValueChangeCheck(); });
        GetScores();
    }


    // Invoked when the value of the text field changes.

    public async void ValueChangeCheck()
    {
        Debug.Log("Value Changed");
        Debug.Log(nickname.text);
        await AddScore(100);
        nickname.interactable = false; 
        GetScores();
    }
    public async Task AddScore(double score)
    {
        var scoreResponse = await LeaderboardsService.Instance.
            AddPlayerScoreAsync(LeaderboardId, score, 
            new AddPlayerScoreOptions { Metadata = new Dictionary<string, string>() { { "player", nickname.text } } });
        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
    }
    // Update is called once per frame
    public async void GetScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { IncludeMetadata = true});
        var res = scoresResponse.Results;
        var list_of_entires = new List<string>();
        foreach (var score in res)
        {
            var pp = score.Metadata;
            Debug.Log(pp);
            list_of_entires.Add(pp);
        }
        var result = string.Join("\n", list_of_entires.ToArray());
        lead.text = result;
    }
    public async void GetPaginatedScores()
    {
        Offset = 10;
        Limit = 10;
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
    }
    
    void Update()
    {
        
    }
}
