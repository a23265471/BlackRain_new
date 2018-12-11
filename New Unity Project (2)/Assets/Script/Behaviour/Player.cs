using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private GameStageData gameStageData;

    private void Awake()
    {
        gameStageData = GameFacade.GetInstance().gameStageData;
    }

    void Start ()
    {
       
      
	}
	
	// Update is called once per frame
	void Update ()
    {
		


	}
}
