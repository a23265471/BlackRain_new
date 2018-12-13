using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Player {

    float[,] ddd;




	// Use this for initialization
	void Start () {
        ddd = new float[,] { { 11, 22, 33 }, { 2, 45,22 } };
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void Move()
    {
        if(Input.GetAxis("Vertical") > 0)
        {
            

        }
    }

}
