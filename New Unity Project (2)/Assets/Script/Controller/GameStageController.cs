using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStageController : MonoBehaviour {

    private GameStageData gameStageData;

    public Transform playerStartPos;
  //  public GameObject player;
    public PlayerBehaviour playerBehaviour;

    public MainCamera_New mainCameraBehaviour;


    private bool CursorLocked;
    private bool GamePause;
    //public GameObject Weapon;

    private void Awake()
    {
        
        gameStageData = GameFacade.GetInstance().gameStageData;
        GameObject mainCamera = Instantiate(gameStageData.CurPlayerStageData.playerData.MainCamera, new Vector3(0,0,0), playerStartPos.rotation);
        GameObject player = Instantiate(gameStageData.CurPlayerStageData.playerData.Player, playerStartPos.position, playerStartPos.rotation);
   //     Weapon= Instantiate(gameStageData.CurPlayerStageData.playerData.Weapon, gameStageData.CurPlayerStageData.playerData.WeaponPos.position, gameStageData.CurPlayerStageData.playerData.WeaponPos.rotation);

     //   Weapon.transform.parent=

        mainCameraBehaviour = mainCamera.GetComponent<MainCamera_New>();
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
        // CursorControl();
        CursorLock();
    }

    private void CursorControl()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && Cursor.visible) 
        {
            Cursor.visible = false;

        }
        else if(Input.GetKeyDown(KeyCode.Tab) && !Cursor.visible)
        {
            Cursor.visible = true;
        }

    }

    private void CursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape))
        {
            if (Screen.lockCursor)
            {
                Screen.lockCursor = false;

            }
            else
            {
                Screen.lockCursor = true;

            }

        }

    }

}
