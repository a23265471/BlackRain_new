using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpine : MonoBehaviour {

    Animator animator;

    public Transform Chest;
    public Transform target;
    Quaternion rotat;
	// Use this for initialization
	void Start ()
    {
        animator = GetComponent<Animator>();
        Chest = animator.GetBoneTransform(HumanBodyBones.RightShoulder);

        rotat = Quaternion.Euler(0, 0, 0);
	}

    // Update is called once per frame
    private void LateUpdate()
    {
       // Chest.rotation = Quaternion.Euler(Chest.rotation., Chest.rotation.y, Chest.rotation.z);
    }
    
}
