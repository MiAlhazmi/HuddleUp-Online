using System;
using System.Collections;
using System.Collections.Generic;
using _Scripts.GameControl;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// This class should be a parent abstract class and every gamemode extends from it
public class TagGameControl : GameModeControl, PlayerToGameControl
{
    private GameControl _gameControl;
    private CtrlPlInteraction _ctrlPlInteraction;
    private SoundEffectsControl _soundEffectCtrl;
    private MusicPlayerControl _musicPlayerControl;
    
    [Header("Players")]
    // [SerializeField] public List<GameObject> _playersList;
    [SerializeField] private List<GameObject> _survivorPlayersList;
    [SerializeField] private List<GameObject> _deadPlayersList;
    [SerializeField] private GameObject currentTagOwner;
    [SerializeField] private GameObject winner;

    private TimerControl timer;
    [Header("Timers")]
    [SerializeField] private GameObject startGameTimerObj;
    [SerializeField] private GameObject gameTimerObj;
    [SerializeField] private GameObject startRoundTimerObj;
    [SerializeField] private GameObject loaderTimerObj;

    private Dictionary<int, int> _scoreboard;

    [SerializeField] private int numberOfRounds;     // determine how many rounds depends on how many player there are
    [SerializeField] private int roundNumber = 0;
    
    [Header("Respawn config")]
    [SerializeField] private float minValueX;
    [SerializeField] private float minValueZ;
    [SerializeField] private float maxValueX;
    [SerializeField] private float maxValueZ;
    [SerializeField] private float minValueY = 10f;
    [SerializeField] private GameObject respawnLand;
    
    [SerializeField] private GameObject canvasGameEnd;
    [SerializeField] private TextMeshProUGUI winnerPlayerName;
    
    private void Awake()
    {
        TryGetComponent(out _gameControl);
        _ctrlPlInteraction = GetComponent<CtrlPlInteraction>();
        _soundEffectCtrl = GetComponent<SoundEffectsControl>();
        _musicPlayerControl = GetComponent<MusicPlayerControl>();

        _scoreboard = _gameControl.scoreboard;   // ToDo: Make sure it takes the address not copies the value (obj)
        timer = GetComponent<TimerControl>();
        
        // _playersList = new List<GameObject>();
        _survivorPlayersList = new List<GameObject>();
        _deadPlayersList = new List<GameObject>();

        ChangePlayersRespawnPos(); 
    }
    
    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }
    

    private void OnSceneChanged(Scene current, Scene next)
    {

    }
    
    private void ChangePlayersRespawnPos()
    {
        respawnLand = null;
        respawnLand = GameObject.FindGameObjectWithTag("Respawn");
    }
    

    /*
     * This function is to be called when the game starts
     */
    private void AddPlayersToSurvivors()
    {
        foreach (var player in _gameControl._playersList)
        {
            _survivorPlayersList.Add(player);
        }  
    }

    private void GivePointsToSurvivors()
    {
        foreach (var player in _survivorPlayersList)
        {
            GiveScore(player.GetInstanceID(), 10 * roundNumber);
        }
    }

    private void UpdateRespawnPos()
    {
        if (respawnLand == null) return;
    
        Transform respawnTransform = respawnLand.transform;
        Vector3 respawnPos = respawnTransform.position;
        Vector3 respawnScale = respawnTransform.localScale;

        minValueX = respawnPos.x - respawnScale.x / 2f;
        maxValueX = respawnPos.x + respawnScale.x / 2f;
        minValueZ = respawnPos.z - respawnScale.z / 2f;
        maxValueZ = respawnPos.z + respawnScale.z / 2f;
        minValueY = respawnPos.y + 5f;
    
    }

    private void GiveRandomPos()
    {
        if (_survivorPlayersList.Count <= 0) return;

        UpdateRespawnPos();
        
        foreach (var player in _survivorPlayersList) 
        {
            player.GetComponent<CharacterController>().enabled = false;     // because CharacterController component won't allow to change the position 
            float xPos = Random.Range(minValueX, maxValueX);
            float zPos = Random.Range(minValueZ, maxValueZ);
            Vector3 pos = new Vector3(xPos, minValueY, zPos);
            player.transform.position = pos;
            player.GetComponent<CharacterController>().enabled = true;
        }
    }
    
    private void GiveRandomPosTo(GameObject player)
    {
        UpdateRespawnPos();
        
        if (_survivorPlayersList.Count <= 0) return;
        
        player.GetComponent<CharacterController>().enabled = false;     // because CharacterController component won't allow to change the position 
        float xPos = Random.Range(minValueX, maxValueX);
        float zPos = Random.Range(minValueZ, maxValueZ);
        Vector3 pos = new Vector3(xPos, minValueY, zPos);
        player.transform.position = pos;
        player.GetComponent<CharacterController>().enabled = true;
        
    }
    
    private void GiveTagRandom()
    {
        if (currentTagOwner != null) return;
        if (_survivorPlayersList.Count <= 0) return;
        
        int randomNumber = Random.Range(0, _survivorPlayersList.Count);
        _survivorPlayersList[randomNumber].GetComponent<Player>().SetIsTagger(true);
        SetTagOwner(_survivorPlayersList[randomNumber]);
        Debug.Log("Tag was given randomly!");
    }
    
    private void DestroyTagger()
    {
        if (currentTagOwner == null) {Debug.LogError("There's no tag owner");return;}
        
        _deadPlayersList.Add(currentTagOwner);
        _survivorPlayersList.Remove(currentTagOwner);
        // ShowPlayerList();
        // currentTagOwner.SetActive(false);   // instead we could deactivate the visuals and change the camera to float camera  -->  Done
        Player taggerPlayerComponent = currentTagOwner.GetComponent<Player>();
        taggerPlayerComponent.DeActivatePlayer();
        taggerPlayerComponent.SetIsTagger(false);
        LockOnSky(currentTagOwner);
        currentTagOwner = null;
    }

    private void LockOnSky(GameObject player)
    {
        // Considering CharacterController is disabled
        float xPos = Random.Range(minValueX, maxValueX);
        float zPos = Random.Range(minValueZ, maxValueZ);
        Vector3 pos = new Vector3(xPos, minValueY + 10f, zPos);
        player.transform.position = pos;
    }

    private void AnnounceWinner()
    {
        if (!winner.IsUnityNull()) winnerPlayerName.text = winner.name;
        timer.StartTimerNumber(4); // this is WinnerAnnouncementTimer // maybe I  don;t need to turn it on
    }
    public void HasHit(GameObject hitter, GameObject target)
    {
        _ctrlPlInteraction.HasHit(hitter, target);
    }
    
    public override void Run()
    {
        numberOfRounds = _gameControl._playersList.Count - 1;
        AddPlayersToSurvivors();
        
        timer.StartTimerNumber(3); // for the loading timer which will call StartGameTimer()
        loaderTimerObj.SetActive(true);
    }

    private void GiveScore(int playerId, int score)
    {
        if (_scoreboard.ContainsKey(playerId))
            _scoreboard[playerId] += score;
        else
            Debug.LogError("player id not found in scoreboard");
    }
    
    public void StartGameTimer()
    {
        ChangePlayersRespawnPos();
        loaderTimerObj.SetActive(false);
        startGameTimerObj.SetActive(true);
        timer.StartTimerNumber(1); // this starts the GameStartingTimer
        GiveRandomPos();
    }
    // StartGame() -> start the timer, unfreeze, call giveTagRandom()
    public void StartGame()
    {
        // ChangePlayerRespawnPos();

        Debug.Log("StartGame() is called");
        
        startGameTimerObj.SetActive(false);
        gameTimerObj.SetActive(true);
        timer.StartTimerNumber(0); // this starts the GameTimer
        GiveRandomPos();
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log("StartRound() is called");
        startRoundTimerObj.SetActive(false);

        if(roundNumber > numberOfRounds) return;
        roundNumber++;
        GiveTagRandom();
        timer.ResetTimer(0);
        timer.StartTimerNumber(0); // this starts the game timer
        _soundEffectCtrl.PlayRoundStart();
        _musicPlayerControl.PlayMapMusic();
    }
    public void EndRound()
    {
        _soundEffectCtrl.PlayRoundEnd();
        _musicPlayerControl.StopMusic();
        Debug.Log("EndRound() is called");
        // Get rid of the tagger!! DestroyTagger(): unity particle system
        DestroyTagger();
        GivePointsToSurvivors();
        if (roundNumber >= numberOfRounds)
        {
            EndGame();
        }
        else
        {
            startRoundTimerObj.SetActive(true);
            timer.StartTimerNumber(2);
        }
    }
    public void EndGame()
    {
        SetWinner();
        AnnounceWinner();
        _gameControl.EndGame();
        canvasGameEnd.SetActive(true);
    }

    // to be called from the WinnerAnnouncementTimer
    public void QuitGame()
    {
        canvasGameEnd.SetActive(false);
        timer.ResetAllTimers();
        QuitGame(false);    // not to main menu
    }
    
    public void QuitGame(bool toMenu)
    {
        if (toMenu)
        {
            _gameControl.QuitGame(toMenu);
        }
        else
        {
            // TODO: safe stats!! 
            _gameControl.QuitGame();
        }
    }
    
    public GameObject GetTagOwner()
    {
        return currentTagOwner;
    }
    
    public void SetTagOwner(GameObject player)
    {
        currentTagOwner = player;
    }

    private void SetWinner()
    {
        winner = _survivorPlayersList[0];
    }
    // GivePointTo(Player): for the crown game to be called every .5 second and it gives 1 point to the crown owner

    private void NotifyAll(String message)
    {
        foreach (var player in _gameControl._playersList)
        {
            player.GetComponent<PlayerUI>().UpdateNotificationText(message);
        }
    }
}
