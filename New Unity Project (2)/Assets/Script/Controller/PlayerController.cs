using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    private GameStageController gameStageController;
    private PlayerBehaviour playerBehaviour;
    private GameStageData gameStageData;
    private InputSetting inputSetting;

    public int moveDirection_Vertical;
    public int moveDirection_Horizontal;

    
    string keepKeyCode;

    IEnumerator cleanKeepKeyCode;

    private void Awake()
    {
        gameStageData = GameFacade.GetInstance().gameStageData;
        gameStageController = GameFacade.GetInstance().gameStageController;
        inputSetting=GameFacade.GetInstance().inputSetting;

      //  Debug.Log((KeyCode)System.Enum.Parse(typeof(KeyCode), "Whatever"));
        
    }

    void Start ()
    {
        playerBehaviour = gameStageController.playerBehaviour;
        keepKeyCode = "";
        cleanKeepKeyCode = null;
        playerBehaviour.playerState = PlayerBehaviour.PlayerState.Move;

    }
	
    private void FixedUpdate()
    {
        PlayerDirectionControl();
        
        if (playerBehaviour.isGround)
        {                      
            Move(playerBehaviour);          
            Avoid(playerBehaviour);
            
        }
        else
        {
           
        }
        Jump(playerBehaviour);
      // Debug.Log(Input.inputString.GetHashCode());
    }

    

    public void Move(PlayerBehaviour player)
    {
        if (playerBehaviour.playerState == PlayerBehaviour.PlayerState.Move)
        {
              player.PlayerMove(moveDirection_Vertical, moveDirection_Horizontal);
            
        }
                   
    }

    public void Avoid(PlayerBehaviour player)
    {
        if (Input.GetKeyDown(inputSetting.inputKey.Avoid) && (moveDirection_Vertical != 0 || moveDirection_Horizontal != 0) && ((playerBehaviour.playerState & PlayerBehaviour.PlayerState.Avoid) != 0)) 
        {           
            player.Avoid();
           // Debug.Log(moveDirection_Vertical);
           
        } 
    }

    public void Jump(PlayerBehaviour player)
    {
        if (Input.GetKeyDown(inputSetting.inputKey.Jump) && ((playerBehaviour.playerState & PlayerBehaviour.PlayerState.Jump) | (playerBehaviour.playerState & PlayerBehaviour.PlayerState.Move)) != 0)   
        {
            player.Jump();
        }
       

    }

    public void Landing(PlayerBehaviour player)
    {
        if ((player.playerState & PlayerBehaviour.PlayerState.Landing) != 0)
        {
            player.Landing();
            Debug.Log("ff");

        }
        
    }

    private void PlayerDirectionControl()
    {     
        if (Input.GetAxis("Vertical") > 0 && (Input.GetKey(inputSetting.inputKey.Forward) || Input.GetKey(KeyCode.UpArrow)))
        {
            moveDirection_Vertical = 1;
        }
        else if (Input.GetAxis("Vertical") < 0 && (Input.GetKey(inputSetting.inputKey.Back) || Input.GetKey(KeyCode.DownArrow)))
        {
            moveDirection_Vertical = -1;
        }
        else
        {
            moveDirection_Vertical = 0;
        }

        if (Input.GetAxis("Horizontal") > 0 && (Input.GetKey(inputSetting.inputKey.Right) || Input.GetKey(KeyCode.RightArrow)))
        {
            moveDirection_Horizontal = 1;
        }
        else if (Input.GetAxis("Horizontal") < 0 && (Input.GetKey(inputSetting.inputKey.Left) || Input.GetKey(KeyCode.LeftArrow)))
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
