using System.Collections;
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

    protected virtual void Displacement(Transform CharactorTransform, float speed,float maxDistance,int moveDirection_Vertical,int moveDirection_Horizontal)
    {
        preTransform = CharactorTransform.position;
        moveControl = MoveControl(CharactorTransform, speed,maxDistance,moveDirection_Vertical,moveDirection_Horizontal);
        
        StopCoroutine(moveControl);
        StartCoroutine(moveControl);
    }

    IEnumerator MoveControl(Transform CharactorTransform,float speed,float maxDis,int moveDirection_Vertical,int moveDirection_Horizontal)
    {
        float MoveZ = moveDirection_Vertical * speed * Time.deltaTime;
        float MoveX = moveDirection_Horizontal * speed * Time.deltaTime;
        moveDis += speed * Time.deltaTime;

        CharactorTransform.Translate(MoveX, 0, MoveZ);
      
        yield return new WaitForSeconds(0.01f);
       
        if (moveDis >= maxDis)
        {
            moveDis = 0;
            StopCoroutine(moveControl);
        }
        else
        {
            moveControl = MoveControl(CharactorTransform,speed, maxDis, moveDirection_Vertical, moveDirection_Horizontal); 
            StopCoroutine(moveControl);
            StartCoroutine(moveControl);        
        }
    }

    protected void AddVerticalForce(Rigidbody rigidbody,float force)
    {
        rigidbody.AddForce(0, force, 0, ForceMode.Impulse);
    }



}
