using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private GameStageController gameStageController;
    private PlayerBehaviour playerBehaviour;
    private GameStageData gameStageData;

    int moveDirection_Vertical;
    int moveDirection_Horizontal;
    string keepKeyCode;

    IEnumerator cleanKeepKeyCode;

    private void Awake()
    {
        gameStageData = GameFacade.GetInstance().gameStageData;
        gameStageController = GameFacade.GetInstance().gameStageController;
        
    }

    void Start ()
    {
        playerBehaviour = gameStageController.playerBehaviour;
        keepKeyCode = "";
        cleanKeepKeyCode = null;

    }
	
	// Update is called once per frame
	void Update ()
    {
      
        

    }
    private void FixedUpdate()
    {
        PlayerDirectionControl();
        if (playerBehaviour.isGround)
        {
            
           // Move(playerBehaviour);
            Avoid(playerBehaviour);
        }
       // Debug.Log("Vertical " + moveDirection_Vertical + "Horizontal " + moveDirection_Horizontal);
    }

    public void Move(PlayerBehaviour player)
    {
         
        player.PlayerMove(moveDirection_Vertical, moveDirection_Horizontal);


    }

    public void Avoid(PlayerBehaviour player)
    {
        if ((moveDirection_Vertical != 0 || moveDirection_Horizontal != 0) && Input.GetKeyDown(KeyCode.LeftShift)) 
        {
           
            player.Avoid(moveDirection_Vertical, moveDirection_Horizontal);
           // Debug.Log(moveDirection_Vertical);
        } 


    }

    private void PlayerDirectionControl()
    {
       
        if (Input.GetAxis("Vertical") > 0 && (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)))
        {
            moveDirection_Vertical = 1;
        }
        else if (Input.GetAxis("Vertical") < 0 && (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)))
        {
            moveDirection_Vertical = -1;
        }
        else
        {
            moveDirection_Vertical = 0;
        }

        if (Input.GetAxis("Horizontal") > 0 && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)))
        {
            moveDirection_Horizontal = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0 && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)))
        {
            moveDirection_Horizontal = -1;
        }
        else
        {
            moveDirection_Horizontal = 0;
        }



           
    }
    
    private void DoKeepCode(ref string keepString,string KeyCode)
    {
        if (keepString == "")
        {
            keepString = KeyCode;
         //   Debug.Log(keepString);
        }
        
    }

    IEnumerator CleanKeepKeyCode()
    {
        yield return new WaitForSeconds(0.2f);
       // Debug.Log("fff");
        keepKeyCode = "";
    }

}
