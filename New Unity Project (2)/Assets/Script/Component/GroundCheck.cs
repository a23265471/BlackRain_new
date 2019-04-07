using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{

    public bool IsGround;
    public bool IsNotStandOnFloor;

    private void Awake()
    {
        IsGround = false;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		

	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            IsGround = true;
            IsNotStandOnFloor = false;
        }
        else
        {
            IsNotStandOnFloor = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
        {
            IsGround = false;
        }
        else
        {
            IsNotStandOnFloor = false;
        }
    }

}
