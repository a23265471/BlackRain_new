using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    private Animator animator;
    private AudioSource audioSource;
    private ParticleSystem particleSystem;

    private IEnumerator moveControl;
    private IEnumerator rigibodyWithAnimationCurve;
    
    private Vector3 preTransform;
    float moveDis;
    float moveTime;
    public bool useGravity;

    float curAnimationCurvePastTime = 0 ; 

    private void Awake()
    {
        moveControl = null;
        rigibodyWithAnimationCurve = null;
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
        
        yield return new WaitForSeconds(0.01f);

        if (Time.time-startTime >= moveTime)
        {
            // moveDis = 0;
            
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

    protected float RunAnimationCurve(AnimationCurve animationCurve, IEnumerator enumerator, float startTime,float endTime,float speed)
    {
        // float endKey=animationCurve.keys[];
        Debug.Log("AniamtionCurve");

        float curTime = startTime + curAnimationCurvePastTime;
        if (curTime >= endTime)
        {
            curTime = endTime;
            curAnimationCurvePastTime = 0;
            useGravity = true;
            StopCoroutine(enumerator);
            Debug.Log("AniamtionCurve_Stop");
        }
        else
        {
            curAnimationCurvePastTime += speed * Time.deltaTime;
            //  readAnimationCurve = ReadAnimationCurve(/*rigidbody,*/ animationCurve, startTime, endTime, speed);
            //  Debug.Log("AniamtionCurve_Start");
            Debug.Log("ss");
            StartCoroutine(enumerator);
        }
        Debug.Log("aa");

        return animationCurve.Evaluate(curTime);
    }

    IEnumerator RigibodyWithAnimationCurve(Rigidbody rigidbody,AnimationCurve animationCurve, float startTime, float endTime, float speed)
    {
        Debug.Log("rr");
        yield return new WaitForSeconds(0.01f);
        Debug.Log("ww");
        rigibodyWithAnimationCurve = RigibodyWithAnimationCurve(rigidbody, animationCurve, startTime, endTime, speed);
        rigidbody.velocity = new Vector3(0, RunAnimationCurve(animationCurve, rigibodyWithAnimationCurve, startTime, endTime, speed), 0);
       // RunAnimationCurve(rigidbody, animationCurve, startTime, endTime, speed);
    }

    protected void RigiBodyMoveWithAniamtionCurve(Rigidbody rigidbody, AnimationCurve animationCurve, float startTime, float endTime, float speed)
    {
        rigibodyWithAnimationCurve = RigibodyWithAnimationCurve(rigidbody, animationCurve, startTime, endTime, speed);
        StartCoroutine(rigibodyWithAnimationCurve);
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
                curVelocity += acceleration*Time.deltaTime;
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
