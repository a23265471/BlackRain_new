using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private GameStageController gameStageController;
    private PlayerBehaviour playerBehaviour;


    private GameStageData gameStageData;

    enum PlayerState
    {
        Move,
        Jump,
        Attack,
        Avoid,
        Damage,
        GetDown,
        GetUp,
        Dead,
        
    }

    private void Awake()
    {
        gameStageData = GameFacade.GetInstance().gameStageData;
        gameStageController = GameFacade.GetInstance().gameStageController;
       
    }

    void Start ()
    {

        playerBehaviour = gameStageController.playerBehaviour;


    }
	
	// Update is called once per frame
	void Update ()
    {

        Move(playerBehaviour);

    }

    public void Move(PlayerBehaviour player)
    {
        float moveDirection_Vertical;
        float moveDirection_Horizontal;

        if(Input.GetAxis("Vertical") == 0)
        {
            moveDirection_Vertical = 0;
        }
        else if (Input.GetAxis("Vertical") > 0)
        {
            moveDirection_Vertical = 1;
        }
        else
        {
            moveDirection_Vertical = -1;

        }

        if (Input.GetAxis("Horizontal") == 0)
        {
            moveDirection_Horizontal = 0;
        }
        else if (Input.GetAxis("Horizontal") > 0)
        {
            moveDirection_Horizontal = 1;
        }
        else
        {
            moveDirection_Horizontal = -1;
        }

        player.PlayerMove(moveDirection_Vertical, moveDirection_Horizontal);

    }

}
