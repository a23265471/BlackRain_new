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
    private bool StopRigibodyWithAnimationCurve;
    public bool RigibodyAnimationCurveIsRunning;

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

    protected virtual void Displacement(Rigidbody rigidbody, Quaternion rotation, float speed,float maxDistance, int moveDirection_X,int moveDirection_Y, int moveDirection_Z)
    {
        // preTransform = CharactorTransform.position;

        moveTime = maxDistance / speed;

        if (moveControl!=null)
        {
            StopCoroutine(moveControl);
            rigidbody.velocity = new Vector3(0, 0, 0);
        }
        moveControl = MoveControl(rigidbody, rotation, Time.time, speed, maxDistance, moveDirection_X, moveDirection_Y, moveDirection_Z);
        
        StartCoroutine(moveControl);
        
    }

    IEnumerator MoveControl(Rigidbody rigidbody,Quaternion rotation,float startTime,float speed,float maxDis, int moveDirection_X, int moveDirection_Y, int moveDirection_Z)
    {
        float MoveX = moveDirection_X * speed;
        float MoveY = moveDirection_Y * speed;
        float MoveZ = moveDirection_Z * speed;
       
        rigidbody.velocity = transform.rotation * new Vector3(MoveX, MoveY, MoveZ);
        yield return new WaitForSeconds(0.01f);

        if (Time.time-startTime >= moveTime)
        {
            // moveDis = 0;
        //     Debug.Log("GG");

            rigidbody.velocity = new Vector3(0, 0, 0);
            

            StopCoroutine(moveControl);            
        }
        else
        {
           // Debug.Log(rigidbody.velocity);

            moveControl = MoveControl(rigidbody, rotation, startTime, speed, maxDis, moveDirection_X, moveDirection_Y, moveDirection_Z);

            StartCoroutine(moveControl);
        }

    }

    protected float AnimationCurve(AnimationCurve animationCurve, float startTime,float endTime,float perLength)
    {

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

    IEnumerator RigibodyRunAnimationCurve_Y(Rigidbody rigidbody, AnimationCurve animationCurve, float startTime, float endTime, float perLength, float perIntervalTime)
    {
     
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, AnimationCurve(animationCurve, startTime, endTime, perLength), rigidbody.velocity.z);

        if (curAnimationCurvePastLong >= endTime)
        {
            curAnimationCurvePastLong = 0;
            RigibodyAnimationCurveIsRunning = false;         
            StopCoroutine(rigibodyWithAnimationCurve);
        }
    
        yield return new WaitForSeconds(perIntervalTime);
        rigibodyWithAnimationCurve = RigibodyRunAnimationCurve_Y(rigidbody, animationCurve, startTime, endTime, perLength, perIntervalTime);

        StartCoroutine(rigibodyWithAnimationCurve);
    }

    protected void RigiBodyMoveWithAniamtionCurve_Y(Rigidbody rigidbody, AnimationCurve animationCurve, float startTime, float endTime, float perIntervalLength, float perIntervalTime)
    {
        float perLength = animationCurve.keys[animationCurve.length - 1].time / perIntervalLength;
        rigibodyWithAnimationCurve = RigibodyRunAnimationCurve_Y(rigidbody, animationCurve, startTime, endTime, perLength, perIntervalTime);
        RigibodyAnimationCurveIsRunning = true;

        StartCoroutine(rigibodyWithAnimationCurve);
    }

    protected void StopRigiBodyMoveWithAniamtionCurve_Y()
    {
        if (RigibodyAnimationCurveIsRunning)
        {
            RigibodyAnimationCurveIsRunning = false;          
            curAnimationCurvePastLong = 0;

            StopCoroutine(rigibodyWithAnimationCurve);
        }
       
    }
    
    protected void AudioPlay(AudioSource audioSource,AudioClip audioClip)
    {
        audioSource.clip = audioClip;
        audioSource.Play();
    }

    protected void ParticlePlay(ParticleSystem particleSystem)
    {
        particleSystem.Stop();
        particleSystem.Play();
    }

    

}
