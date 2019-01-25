using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStageController : MonoBehaviour {

    private GameStageData gameStageData;

    public Transform playerStartPos;
    public GameObject player;
    public PlayerBehaviour playerBehaviour;

    private void Awake()
    {
        gameStageData = GameFacade.GetInstance().gameStageData;
        player = Instantiate(gameStageData.CurPlayerStageData.playerData.Player, playerStartPos.position, playerStartPos.rotation);
        playerBehaviour = player.GetComponent<PlayerBehaviour>();
        
    }
    // Use this for initialization
    void Start ()
    {
       
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

    }
	
	// Update is called once per frame
	void Update ()
    {
        CursorControl();

    }

    private void CursorControl()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl) && Cursor.visible) 
        {
            Cursor.visible = false;

        }
        else if(Input.GetKeyDown(KeyCode.LeftControl) && !Cursor.visible)
        {
            Cursor.visible = true;
        }

    }

}
