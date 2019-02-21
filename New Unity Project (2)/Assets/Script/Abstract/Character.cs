﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;

    private IEnumerator moveControl;
    
    private Vector3 preTransform;
    float moveDis;
    float moveTime;
    public bool useGravity;

    float aa = 1;

    private void Awake()
    {
        moveControl = null;     
    }

    protected virtual void AnimationBlendTreeControll(Animator animator,string parameterName, float targetValue,ref float controllValue,float animationSpeed)
    {
        controllValue = Mathf.Lerp(controllValue, targetValue, animationSpeed);

        if (controllValue<=(targetValue+0.04f) && controllValue >= (targetValue - 0.04f))
        {
            controllValue = targetValue;
        }   
        
        animator.SetFloat(parameterName, controllValue);
    } 

    protected virtual void AnimationTrigger(Animator animator, string parameterName)
    {
        animator.SetTrigger(parameterName);
    }

    protected virtual void Displacement(Rigidbody rigidbody, Quaternion rotation, float speed,float maxDistance, int moveDirection_X,int moveDirection_Y, int moveDirection_Z,bool isGravity)
    {
        // preTransform = CharactorTransform.position;
        useGravity = isGravity;

        moveTime = maxDistance / speed;
        
        moveControl = MoveControl(rigidbody, rotation, Time.time, speed, maxDistance, moveDirection_X, moveDirection_Y, moveDirection_Z);
        
        StopCoroutine(moveControl);
        StartCoroutine(moveControl);
        Debug.Log(moveTime);
    }

    IEnumerator MoveControl(Rigidbody rigidbody,Quaternion rotation,float startTime,float speed,float maxDis, int moveDirection_X, int moveDirection_Y, int moveDirection_Z)
    {
        float MoveX = moveDirection_X * speed;
        float MoveY = moveDirection_Y * speed;
        float MoveZ = moveDirection_Z * speed;
       

        rigidbody.velocity = rotation * new Vector3(MoveX, MoveY, MoveZ);

      /*  aa += 1;

         moveDis += (speed+ aa) * Time.deltaTime;
         moveDis = Mathf.Clamp(moveDis, 0, maxDis);
         */
       // Debug.Log(rigidbody.velocity);

        
        yield return new WaitForSeconds(0.01f);

        if (Time.time-startTime >= moveTime)
        {
            // moveDis = 0;
            Debug.Log("Stop");
            rigidbody.velocity = new Vector3(0, 0, 0);
            useGravity = true;

            StopCoroutine(moveControl);            
        }
        else
        {
            moveControl = MoveControl(rigidbody, rotation, startTime, speed, maxDis, moveDirection_X, moveDirection_Y, moveDirection_Z);
            StartCoroutine(moveControl);
        }

    }



    protected void AddVerticalForce(Rigidbody rigidbody,float force)
    {
        rigidbody.AddForce(0, force, 0, ForceMode.Impulse);
    }

    protected void Gravity(Rigidbody rigidbody,bool isGrounded,float maxVelocity,ref float curVelocity,float acceleration)
    {
        
        if (!isGrounded)
        {
            if (curVelocity >= maxVelocity)
            {
                curVelocity = maxVelocity;
            }
            else
            {
                curVelocity += acceleration;
            }

            rigidbody.velocity = Vector3.down * curVelocity;
           // Debug.Log("gravity");

        }
        else
        {
            if (curVelocity != 0)
            {
                curVelocity = 0;
            }
        }

       // Debug.Log(curVelocity);
    }

}
