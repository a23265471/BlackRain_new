using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFacade : MonoBehaviour {

    private static GameFacade instance;
  
    public GameFacade GetInstance()
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

    #endregion


    #region Models
    public PlayerStageData playerStageData;

    #endregion



    private void Initialize()
    {

        playerStageData = new PlayerStageData();

    }

    private void Awake()
    {
        GetInstance();

    }
}
