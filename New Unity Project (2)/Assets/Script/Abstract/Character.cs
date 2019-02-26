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
    private bool StopRigibodyWithAnimationCurve;
    private bool RigibodyAnimationCurveIsRunning;

    public float curAnimationCurvePastLong = 0 ; 

    private void Awake()
    {
        moveControl = null;
        rigibodyWithAnimationCurve = null;
        StopRigibodyWithAnimationCurve = false;
        RigibodyAnimationCurveIsRunning = false;
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
            

            StopCoroutine(moveControl);            
        }
        else
        {
            moveControl = MoveControl(rigidbody, rotation, startTime, speed, maxDis, moveDirection_X, moveDirection_Y, moveDirection_Z);

            StartCoroutine(moveControl);
        }

    }

    protected float AnimationCurve(AnimationCurve animationCurve, float startTime,float endTime,float perLength)
    {
      ///  Debug.Log("AniamtionCurve");

        float curTime = startTime + curAnimationCurvePastLong;

        if (curTime >= endTime)
        {
            curTime = endTime;

        }
        else
        {
            curAnimationCurvePastLong += perLength;
        }

        return animationCurve.Evaluate(curTime);
    }

    IEnumerator RigibodyRunAnimationCurve(Rigidbody rigidbody, AnimationCurve animationCurve, float startTime, float endTime, float perLength, float perIntervalTime)
    {
     
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, AnimationCurve(animationCurve, startTime, endTime, perLength), rigidbody.velocity.z);

        if (curAnimationCurvePastLong >= endTime)
        {

            curAnimationCurvePastLong = 0;
            useGravity = true;
            RigibodyAnimationCurveIsRunning = false;         
            StopCoroutine(rigibodyWithAnimationCurve);
        }
    
        yield return new WaitForSeconds(perIntervalTime);
        rigibodyWithAnimationCurve = RigibodyRunAnimationCurve(rigidbody, animationCurve, startTime, endTime, perLength, perIntervalTime);

        StartCoroutine(rigibodyWithAnimationCurve);
    }

    protected void RigiBodyMoveWithAniamtionCurve(Rigidbody rigidbody, AnimationCurve animationCurve, float startTime, float endTime, float perIntervalLength, float perIntervalTime)
    {
        float perLength = animationCurve.keys[animationCurve.length - 1].time / perIntervalLength;
        rigibodyWithAnimationCurve = RigibodyRunAnimationCurve(rigidbody, animationCurve, startTime, endTime, perLength, perIntervalTime);
        RigibodyAnimationCurveIsRunning = true;

        StartCoroutine(rigibodyWithAnimationCurve);
    }

    protected void StopRigiBodyMoveWithAniamtionCurve()
    {
        if (RigibodyAnimationCurveIsRunning)
        {
            RigibodyAnimationCurveIsRunning = false;          
            curAnimationCurvePastLong = 0;

            StopCoroutine(rigibodyWithAnimationCurve);
        }
       
    }

   /* protected void AddVerticalForce(Rigidbody rigidbody,float force)
    {
        rigidbody.AddForce(0, force, 0, ForceMode.Impulse);
    }*/

   /* protected void Gravity(Rigidbody rigidbody,bool isGrounded,float maxVelocity,ref float curVelocity,float acceleration)
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
    }*/

}
