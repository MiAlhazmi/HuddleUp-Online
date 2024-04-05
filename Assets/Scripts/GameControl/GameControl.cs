using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

// This class should be a parent abstract class and every gamemode extends from it
public class GameControl : MonoBehaviour, PlayerToGameControl
{
    private CtrlPlInteraction _ctrlPlInteraction;

    private TimerControl timer;
    [SerializeField] private GameObject startGameTimerObj;
    [SerializeField] private GameObject gameTimerObj;
    
    private List<GameObject> _playersList;

    [SerializeField] private GameObject currentTagOwner;
    // Scoreboard

    [SerializeField] private float minValueX;
    [SerializeField] private float minValueZ;
    [SerializeField] private float maxValueX;
    [SerializeField] private float maxValueZ;

    [SerializeField] private GameObject canvasGameEnd;
    private void Awake()
    {
        _ctrlPlInteraction = GetComponent<CtrlPlInteraction>();
        timer = GetComponent<TimerControl>();
    }

    private void OnEnable()
    {
        // UnityEngine.InputSystem.PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    public void OnPlayerJoined(Player player)
    {
        Debug.Log("From GameControl OnPlayerJoin(): " + player.GetInstanceID());
    }

    // Start is called before the first frame update
    void Start()
    {
        _playersList = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        ShowPlayerList();
    }
    
    public void AddPlayer()
    {
        // I can search by instanceId for every player instance in the game
        foreach (GameObject playerObj in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log("Inside FindGameObjectsWithTag(\"Player\") loop");
            if (!_playersList.Contains(playerObj))
            {
                Debug.Log("New player game obj found!");
                _playersList.Add(playerObj);
            }
        }
        
        // GameObject playerGameObject = GameObject.Find("Player(Clone)");
        // if (!_playersList.Contains(playerGameObject) && playerGameObject != null){
        //     _playersList.Add(playerGameObject);
        // }
        
        // Debug.Log(UnityEngine.InputSystem.PlayerInputManager.instance.joinAction.reference.GameObject().name);
        // UnityEngine.InputSystem.PlayerInputManager.instance.playerJoinedEvent.AddListener(UnityAction<>);
    }

    public void ShowPlayerList()
    {
        Debug.Log("Number of players: " + _playersList.Count);
        foreach (var elem in _playersList)
        {
            Debug.Log("player instance id: " + elem.GetInstanceID());
        }
    }
    // StartTimer()
    public void StartGameTimer()
    {
        startGameTimerObj.SetActive(true);
        timer.StartTimerNumber(1); // this starts the game starting timer
        gameTimerObj.SetActive(true);
    }
    // StartGame() -> start the timer, unfreeze, call giveTagRandome()
    public void StartGame()
    {
        startGameTimerObj.SetActive(false);
        GiveRandomPos();
        GiveTagRandom();
        timer.StartTimerNumber(0); // this starts the game timer
        
    }
    // EndGame()
    public void EndGame()
    {
        canvasGameEnd.SetActive(true);
        Time.timeScale = 0;
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
        foreach (var player in _playersList)
        {
            player.GetComponent<CharacterController>().gameObject.SetActive(false);
            float xPos = Random.Range(minValueX, maxValueX);
            float zPos = Random.Range(minValueZ, maxValueZ);
            player.transform.position = new Vector3(xPos, 10f, zPos);
            player.GetComponent<CharacterController>().gameObject.SetActive(true);
        }
        
    }
    public void GiveTagRandom()
    {
        if (currentTagOwner != null) return;
        if (_playersList.Count <= 0) return;
        
        int randomNumber = Random.Range(0, _playersList.Count);
        _playersList[randomNumber].GetComponent<Player>().SetIsTagger(true);
        SetTagOwner(_playersList[randomNumber]);
        Debug.Log("Tag was given randomly!");
    }
    
    public void HasHit(GameObject hitter, GameObject target)
    {
        _ctrlPlInteraction.HasHit(hitter, target);
    }
    
    public GameObject GetTagOwner()
    {
        return currentTagOwner;
    }
    
    public void SetTagOwner(GameObject player)
    {
        currentTagOwner = player;
    }
    
    
    // GivePointTo(Player): for the crown game to be called every .5 second and it gives 1 point to the crown owner 
}
