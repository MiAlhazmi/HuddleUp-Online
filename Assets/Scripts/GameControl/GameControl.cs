using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

// This class should be a parent abstract class and every gamemode extends from it
public class GameControl : MonoBehaviour, PlayerToGameControl
{
    public static GameControl Instance;
    
    private CtrlPlInteraction _ctrlPlInteraction;

    private TimerControl timer;
    [SerializeField] private GameObject startGameTimerObj;
    [SerializeField] private GameObject gameTimerObj;
    [SerializeField] private GameObject startRoundTimerObj;
    [SerializeField] private GameObject loaderTimerObj;
    [SerializeField] private GameObject pauseMenuObj;
    private PauseMenu pauseMenu;

    
    private List<GameObject> _playersList;

    [SerializeField] private GameObject currentTagOwner;
    private GameObject winner;
    // Scoreboard

    [SerializeField] private int numberOfRounds;     // determine how many rounds depends on how many player there are
    [SerializeField] private int roundNumber = 0;
    
    // Todo: Will change this with a plane for respawns
    [SerializeField] private float minValueX;
    [SerializeField] private float minValueZ;
    [SerializeField] private float maxValueX;
    [SerializeField] private float maxValueZ;
    [SerializeField] private float minValueY = 10f;
    [SerializeField] private GameObject respawnLand;
    
    [SerializeField] private GameObject canvasGameEnd;
    [SerializeField] private TextMeshProUGUI winnerPlayerName;
    private PlayerInputManager playerInputManager;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        _ctrlPlInteraction = GetComponent<CtrlPlInteraction>();
        timer = GetComponent<TimerControl>();
        _playersList = new List<GameObject>();
        playerInputManager = GetComponent<PlayerInputManager>();

        pauseMenu = pauseMenuObj.GetComponent<PauseMenu>();
        ChangePlayerRespawnPos();
    }
    
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += PlayerInputManagerOnPlayerJoined;
    }
    private void OnDisable()
    {
        if (playerInputManager != null)
            playerInputManager.onPlayerJoined -= PlayerInputManagerOnPlayerJoined;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMenu.isPaused)
            {
                pauseMenu.Resume();
            }
            else
            {
                pauseMenu.Pause();
            }
        }
    }

    public void OnStartButtonClicked()
    {
        foreach (var player in _playersList)
        {
            DontDestroyOnLoad(player);
        }

        // var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // if (SceneManager.sceneCount > nextSceneIndex)
        // {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        // }
        timer.StartTimerNumber(3); // for the loading timer
        // ChangePlayerRespawnPos();
        // StartGameTimer();
    }

    private void ChangePlayerRespawnPos()
    {
        Debug.Log("Looking for a respawn land... ");
        respawnLand = null;
        respawnLand = GameObject.FindGameObjectWithTag("Respawn");
    }
    private void PlayerInputManagerOnPlayerJoined(PlayerInput playerInput)
    {
        AddPlayer(playerInput.GameObject());
        playerInput.GameObject().name = $"Player{_playersList.Count}";
        Debug.Log("From GameControl OnPlayerJoin(): " + playerInput.GameObject().name + " Joined the lobby");
        GiveRandomPosTo(playerInput.GameObject());
    }


    private void AddPlayer(GameObject player)
    {
        _playersList.Add(player);
    }
    
    // public void AddPlayer()
    // {
    //     // I can search by instanceId for every player instance in the game
    //     foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
    //     {
    //         Debug.Log("Inside FindGameObjectsWithTag(\"Player\") loop");
    //         if (!_playersList.Contains(playerObj))
    //         {
    //             Debug.Log("New player game obj found!");
    //             _playersList.Add(playerObj);
    //         }
    //     }
    //     // GameObject playerGameObject = GameObject.Find("Player(Clone)");
    //     // if (!_playersList.Contains(playerGameObject) && playerGameObject != null){
    //     //     _playersList.Add(playerGameObject);
    //     // }
    //     
    //     // Debug.Log(UnityEngine.InputSystem.PlayerInputManager.instance.joinAction.reference.GameObject().name);
    //     // UnityEngine.InputSystem.PlayerInputManager.instance.playerJoinedEvent.AddListener(UnityAction<>);
    // }

    public void ShowPlayerList()
    {
        Debug.Log("Number of players: " + _playersList.Count);
        foreach (var elem in _playersList)
        {
            Debug.Log(elem.name);
        }
    }
    // HasHit(HitterPLayerGameObject, TargetPLayerGameObject) -> check if the hitter is a tagger
    // GiveTagTo(Player, fromPlayer)
    // CheckTagOwner()
    // GivePointsToSurvivors(): to be called after a tagger dies
    // JoinPlayer()
    // KickPlayer()
    // FillBots

    private void GiveRandomPos()
    {
        Debug.Log("player list count: " + _playersList.Count);
        if (_playersList.Count <= 0) return;

        if (respawnLand != null)
        {
            Transform respawnTransform = respawnLand.transform;
            Vector3 respawnPos = respawnTransform.position;
            Vector3 respawnScale = respawnTransform.localScale;

            minValueX = respawnPos.x - respawnScale.x / 2f;
            maxValueX = respawnPos.x + respawnScale.x / 2f;
            minValueZ = respawnPos.z - respawnScale.z / 2f;
            maxValueZ = respawnPos.z + respawnScale.z / 2f;
            minValueY = respawnPos.y + 10f;
        }
        
        foreach (var player in _playersList)
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
        Debug.Log("player list count: " + _playersList.Count);
        if (_playersList.Count <= 0) return;
        
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
        if (_playersList.Count <= 0) return;
        
        int randomNumber = Random.Range(0, _playersList.Count);
        _playersList[randomNumber].GetComponent<Player>().SetIsTagger(true);
        SetTagOwner(_playersList[randomNumber]);
        Debug.Log("Tag was given randomly!");
    }
    

    private void DestroyTagger()
    {
        if (currentTagOwner == null) return;
        
        _playersList.Remove(currentTagOwner);
        ShowPlayerList();
        currentTagOwner.SetActive(false);
        currentTagOwner = null;
    }

    private void AnnounceWinner()
    {
        if (!winner.IsUnityNull()) winnerPlayerName.text = winner.name;
        timer.StartTimerNumber(4); // maybe I don;t need to turn it on
        
    }
    public void HasHit(GameObject hitter, GameObject target)
    {
        _ctrlPlInteraction.HasHit(hitter, target);
    }
    

    public void StartGameTimer()
    {
        ChangePlayerRespawnPos();
        loaderTimerObj.SetActive(false);
        startGameTimerObj.SetActive(true);
        timer.StartTimerNumber(1); // this starts the GameStartingTimer
        numberOfRounds = _playersList.Count - 1;
        playerInputManager.DisableJoining();
        GiveRandomPos();
    }
    // StartGame() -> start the timer, unfreeze, call giveTagRandom()
    public void StartGame()
    {
        ChangePlayerRespawnPos();

        Debug.Log("StartGame() is called");
        // if (numberOfRounds < 1) // Todo: should be 1 not 0
        // {
        //     Debug.Log("There are no enough players!");
        //     playerInputManager.EnableJoining();
        //     return;
        // }
        
        startGameTimerObj.SetActive(false);
        gameTimerObj.SetActive(true);
        GiveRandomPos();
        StartRound();
    }

    public void StartRound()
    {
        Debug.Log("StartRound() is called");
        startRoundTimerObj.SetActive(false);

        // if(roundNumber > numberOfRounds) return;
        roundNumber++;
        GiveTagRandom();
        timer.ResetTimer(0);
        timer.StartTimerNumber(0); // this starts the game timer
    }
    public void EndRound()
    {
        Debug.Log("EndRound() is called");
        // Get rid of the tagger!! DestroyTagger(): unity particle system
        DestroyTagger();
        // GivePointsToSurvivors
        if (roundNumber >= numberOfRounds)
        {
            EndGame();
        }
        else
        {
            startRoundTimerObj.SetActive(true);
            timer.StartTimerNumber(2);
            // StartRound();
        }
    }
    public void EndGame()
    {
        SetWinner();
        AnnounceWinner();
        canvasGameEnd.SetActive(true);
        // Time.timeScale = 0;
    }

    public void QuitGame()
    {
        var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCount > nextSceneIndex)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            // Destroy(gamecontrol); & players
            SceneManager.LoadScene("MainMenu_Scene");
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
        winner = _playersList[0];
    }
    // GivePointTo(Player): for the crown game to be called every .5 second and it gives 1 point to the crown owner 
}
