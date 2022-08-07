using DigitalRubyShared;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder : Singleton<Finder>
{
    private GameObject gameManagerObj;

    private GameManager gameManager;
    private PrafabHolder prafabHolder;
    private GameState gameState;
    private Player player;
    private TimeController timeController;


    private GameObject GetGameManagerObj() {
        if (gameManagerObj == null) {
            gameManagerObj = GameObject.FindGameObjectWithTag("GameController");
        }
        return gameManagerObj;
    }
    public GameManager GetGameManager() {
        if (gameManager == null) {
            gameManager = GetGameManagerObj().GetComponent<GameManager>();
        }

        return gameManager;
    }

    public PrafabHolder GetPrafabHolder() {
        if (prafabHolder == null) {
            prafabHolder = GetGameManagerObj().GetComponent<PrafabHolder>();
            prafabHolder.GetType();
        }
        return prafabHolder;
    }

    public GameState GetGameState() {
        if (gameState == null) {
            gameState = GetGameManagerObj().GetComponent<GameState>();
        }
        return gameState;
    } 

    public TimeController GetTimeController()
    {
        if (timeController == null) {
            timeController = GetGameManagerObj().GetComponent<TimeController>();
        }
        return timeController;
    }

    public Player GetPlayer() {
        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag(TagMapping.Player.ToString());
            if (playerObj != null) {
                player = playerObj.GetComponent<Player>();
            }
        }

        return player;
    }
   
    private GameObject FindObjectWithComponent(GameObject[] objects, System.Type systemType) {
        foreach (GameObject gameObj in objects) {
            if (gameObj.GetComponent(systemType) != null) {
                return gameObj;
            }
        }

        return null;
    }

    protected virtual void Start()
    {
    }
}
