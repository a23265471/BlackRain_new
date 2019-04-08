using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShadowSwitch : MonoBehaviour {
   public bool GhostShadowIsDoing = false;


    void Start () {

    }
	
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space) && !GhostShadowIsDoing)
        {
            this.gameObject.GetComponent<GhostShadow>().enabled = true;
            Debug.Log("open");
            GhostShadowIsDoing = true;
        }
        else if(Input.GetKeyDown(KeyCode.Space) && GhostShadowIsDoing)
        {
            this.gameObject.GetComponent<GhostShadow>().enabled = false;
            Debug.Log("close");
            GhostShadowIsDoing = false;
        }
    }
}
