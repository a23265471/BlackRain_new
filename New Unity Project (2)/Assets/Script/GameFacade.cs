using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(GameStageController))]

public class GameFacade : MonoBehaviour {

    private static GameFacade instance;
  
    public static GameFacade GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindObjectOfType<GameFacade>();
            if (instance == null)
            {
                throw new System.Exception("GameFacade不存在於場景中，請在場景中添加");
            }
            instance.Initialize();
        }
        return instance;
    }



    #region Controller
    public PlayerController playerController;
    public GameStageController gameStageController;
   

    #endregion


    #region Models   
    public PlayerStageData[] playerStageData;  
    public GameStageData gameStageData;
    public InputSetting inputSetting;
    #endregion

    

    private void Initialize()
    {
        playerController = GetComponent<PlayerController>();
        gameStageController = GetComponent<GameStageController>();

        

        gameStageData = new GameStageData();
        
    }

    private void Awake()
    {
        GetInstance();

    }

    private void Start()
    {
        
    }
}
