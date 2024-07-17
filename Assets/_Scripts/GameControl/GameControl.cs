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
public class GameControl : MonoBehaviour, PlayerToGameControl
{
    public static GameControl Instance;

    private GameModeControl _sellectedGameMode;
    private List<GameModeControl> _gameModes;
    private CtrlPlInteraction _ctrlPlInteraction;
    private MapSelectionControl _mapSelectionCtrl;
    private SoundEffectsControl _soundEffectCtrl;
    private MusicPlayerControl _musicPlayerControl;
    
    [SerializeField] private Camera gameCamera;
    
    [Header("Players")]
    [SerializeField] public List<GameObject> _playersList;
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
    
    [SerializeField] private GameObject pauseMenuObj;
    private PauseMenu pauseMenu;
    
    // Scoreboard -> key : value;   playerObj or id : score
    public Dictionary<int, int> scoreboard { get; set; } = new Dictionary<int, int>(); // ToDo: public get private set

    [SerializeField] private int _numGamesPlayed;
    [SerializeField] private int numberOfRounds;     // determine how many rounds depends on how many player there are
    [SerializeField] private int roundNumber = 0;
    
    // Todo: Will change this with a plane for respawns, I think this is done √
    [Header("Respawn config")]
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

        TryGetComponent(out _sellectedGameMode);
        _ctrlPlInteraction = GetComponent<CtrlPlInteraction>();
        _mapSelectionCtrl = GetComponent<MapSelectionControl>();
        _soundEffectCtrl = GetComponent<SoundEffectsControl>();
        _musicPlayerControl = GetComponent<MusicPlayerControl>();
        
        timer = GetComponent<TimerControl>();
        pauseMenu = pauseMenuObj.GetComponent<PauseMenu>();
        
        _playersList = new List<GameObject>();
        _survivorPlayersList = new List<GameObject>();
        _deadPlayersList = new List<GameObject>();
        playerInputManager = GetComponent<PlayerInputManager>();

        ChangeRespawnPos(); 
        ChangeGameMode(GameMode.Tag);
    }
    
    private void OnEnable()
    {
        playerInputManager.onPlayerJoined += PlayerInputManagerOnPlayerJoined;
        SceneManager.activeSceneChanged += OnSceneChanged;
    }
    private void OnDisable()
    {
        if (playerInputManager != null)
            playerInputManager.onPlayerJoined -= PlayerInputManagerOnPlayerJoined;
        SceneManager.activeSceneChanged -= OnSceneChanged;
        scoreboard.Clear();
        scoreboard = null;  // We need to clear dictionaries when not reloading domain and scene every play in the editor
    }

    private void PlayerInputManagerOnPlayerJoined(PlayerInput playerInput)
    {
        gameCamera.gameObject.SetActive(false);
        AddPlayer(playerInput.GameObject());
        playerInput.GameObject().name = $"Player{_playersList.Count}";
        Debug.Log("From GameControl OnPlayerJoin(): " + playerInput.GameObject().name + " Joined the lobby");
        GiveRandomPosTo(playerInput.GameObject());
    }

    private void OnSceneChanged(Scene current, Scene next)
    {
        ChangeRespawnPos();
        // if scene is lobby: reset all player (recover all dead, empty dead list, empty survivor list) 
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("PlaygroundScene") && _numGamesPlayed != 0) // <- instead we can check current var, if !MainMenu and then we increment numGames... here 
        {
            ResetAllPlayers();
            currentTagOwner = null;
            winner = null;
            winnerPlayerName.text = "";
            timer.ResetAllTimers();
            startGameTimerObj.SetActive(false);
            gameTimerObj.SetActive(false);
            startRoundTimerObj.SetActive(false);
            loaderTimerObj.SetActive(false);
            canvasGameEnd.SetActive(false);
            numberOfRounds = 0;
            roundNumber = 0;
        }
    }
    private void Update()
    {
        // ToDo: Delete this and have it in PLayerControllerMap
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
    
    private void ResetAllPlayers()
    {
        // (recover all dead, empty dead list, empty survivor list) 
        foreach (var player in _playersList)
        {
            // player.SetActive(); nope, not anymore!
            // activate visuals only
            player.GetComponent<Player>().ActivatePlayer();
        }

        _survivorPlayersList.Clear();
        _deadPlayersList.Clear();
        GiveRandomPos();
    }
    private void ChangeRespawnPos()
    {
        respawnLand = null;
        respawnLand = GameObject.FindGameObjectWithTag("Respawn");
    }

    private void AddPlayer(GameObject player)
    {
        _playersList.Add(player);
        if (!scoreboard.ContainsKey(player.GetInstanceID()))  // new
            scoreboard.Add(player.GetInstanceID(), 0);
    }

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
        Debug.Log("player list count: " + _playersList.Count);
        if (_playersList.Count <= 0) return;

        UpdateRespawnPos();
        
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
        UpdateRespawnPos();
        
        if (_playersList.Count <= 0) return;
        
        player.GetComponent<CharacterController>().enabled = false;     // because CharacterController component won't allow to change the position 
        float xPos = Random.Range(minValueX, maxValueX);
        float zPos = Random.Range(minValueZ, maxValueZ);
        Vector3 pos = new Vector3(xPos, minValueY, zPos);
        player.transform.position = pos;
        player.GetComponent<CharacterController>().enabled = true;
        
    }
    
    private void LockOnSky(GameObject player)
    {
        // Considering CharacterController is disabled
        float xPos = Random.Range(minValueX, maxValueX);
        float zPos = Random.Range(minValueZ, maxValueZ);
        Vector3 pos = new Vector3(xPos, minValueY + 10f, zPos);
        player.transform.position = pos;
    }
    
    public void HasHit(GameObject hitter, GameObject target)
    {
        _ctrlPlInteraction.HasHit(hitter, target);
    }

    public void ChangeGameMode(GameMode gameMode)
    {
        switch (gameMode)
        {
            case GameMode.Tag:
                if (TryGetComponent(out TagGameControl tagGameControl))
                    _sellectedGameMode = tagGameControl;
                break;
            case GameMode.Crown:
                break;
        }

        if (_sellectedGameMode.IsUnityNull())
            _sellectedGameMode = GetComponent<TagGameControl>();
    }
    public void OnStartButtonClicked()
    {
        numberOfRounds = _playersList.Count - 1;
        if (numberOfRounds < 1)
        {
            NotifyAll("more players are needed");
            return;
        }

        foreach (var player in _playersList)
        {
            DontDestroyOnLoad(player);
        }

        SceneManager.LoadScene(_mapSelectionCtrl.selectedMap);
        
        // All timer calls are now called from the game mode controllers
        // timer.StartTimerNumber(3); // for the loading timer which will call StartGameTimer()
        // loaderTimerObj.SetActive(true);
        _sellectedGameMode.Run();
    }

    public void EndGame()
    {
        _numGamesPlayed++;
    }

    // to be called from the WinnerAnnouncementTimer
    public void QuitGame()
    {
        canvasGameEnd.SetActive(false);
        // timer.ResetAllTimers();
        QuitGame(false);    // not to main menu
    }
    
    public void QuitGame(bool toMenu)
    {
        // var nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        // if (SceneManager.sceneCount > nextSceneIndex)
        // {
        //     SceneManager.LoadScene(nextSceneIndex);
        // }
        if (toMenu)
        {
            // Destroy GameControl & players
            DestroyAllObjects();
            SceneManager.LoadScene("MainMenu_Scene");
            Destroy(gameObject);    // destroy GameControl
        }
        else
        {
            // TODO: safe stats!! 
            SceneManager.LoadScene("PlaygroundScene");
        }
    }

    private void DestroyAllObjects()
    {
        Debug.Log("All objects Destroyed");
        foreach (var player in _playersList)
        {
            Destroy(player);
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
    
    private void NotifyAll(String message)
    {
        foreach (var player in _playersList)
        {
            player.GetComponent<PlayerUI>().UpdateNotificationText(message);
        }
    }
}
