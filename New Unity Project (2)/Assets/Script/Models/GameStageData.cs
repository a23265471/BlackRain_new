using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStageData
{
    public PlayerStageData CurPlayerStageData
    {
        get
        {
            PlayerStageData[] curPlayerData = GameFacade.GetInstance().playerStageData;

            return curPlayerData[0];
        }


    } 

	


}
