using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gravity : MonoBehaviour
{
    public bool UseGravity;
    public GameObject GroundCheckObject;
    public AnimationCurve GravityCurve;

    public float GravityPerIntervalTime;

    private bool isGravity;
    private float currentTime;
    private float currentVelocityY;
    public GroundCheck groundCheck;
    private Rigidbody rigidbody;

   

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        groundCheck = GroundCheckObject.GetComponent<GroundCheck>();
        currentTime = -GravityPerIntervalTime;
        UseGravity = true;
    }

    private void FixedUpdate()
    {
        if (UseGravity)
        {
           // Debug.Log(rigidbody.velocity.y);
            if (!groundCheck.IsGround)
            {
                /* if (!groundCheck.IsNotStandOnFloor)
                 {
                     if (rigidbody.velocity.y <= 0.5f && rigidbody.velocity.y >= -0.5f)
                     {

                         isGravity = false;

                     }

                 }
                 StartUseGravity();*/

                
                    Fall();
               
            }
            
        }
    }

    public void Fall()
    {
        /*   if (!isGravity)
           {
               isGravity = true;
               StopRigiBodyMoveWithAniamtionCurve_Y();
               Keyframe gravityKey = playerParameter.jumpParameter.GravityCurve.keys[playerParameter.jumpParameter.GravityCurve.keys.Length - 1];
               RigiBodyMoveWithAniamtionCurve_Y(playerRigidbody, playerParameter.jumpParameter.GravityCurve, 0, gravityKey.time, 12, playerParameter.jumpParameter.GravityPerIntervalTime);

           }*/
        
        if(currentTime >= GravityCurve.keys[GravityCurve.length - 1].time)
        {
            currentTime = -GravityPerIntervalTime; 
            
        }
        else
        {
            currentTime += GravityPerIntervalTime;
            currentVelocityY = GravityCurve.Evaluate(currentTime);
        }

        rigidbody.velocity = new Vector3(rigidbody.velocity.x, currentVelocityY, rigidbody.velocity.z);
       
    }

    public void StartUseGravity()
    {
        UseGravity = true;
    }

    public void StopUseGravity()
    {
        currentTime = -GravityPerIntervalTime;
        UseGravity = false;
    }

    public void StopGroundCheck()
    {
        GroundCheckObject.SetActive(false);
    }

    public void StartGroundCheck()
    {
        GroundCheckObject.SetActive(true);
    }
    //private void 
         

}
