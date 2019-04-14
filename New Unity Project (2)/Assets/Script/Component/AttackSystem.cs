using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Gravity))]
[RequireComponent(typeof(AnimationHash))]
[RequireComponent(typeof(Animator))]
public class AttackSystem : MonoBehaviour
{
    public SkillList skillList;
    private Animator animator;
    private AnimationHash animationHash;
    private Gravity gravity;
    public Dictionary<int, SkillList.AttackParameter> AttackCollection;


    private bool CanTriggerNextAttack;
    public bool isTriggerAttack;
    private SkillList.AttackParameter currentAttackInfo;

    public bool IsAttack;

    IEnumerator detectAttackStateForceExit;

    private void Awake()
    {
        CreateAttackCollection();
        animator = GetComponent<Animator>();
        animationHash = GetComponent<AnimationHash>();
        gravity = GetComponent<Gravity>();
    }

    void Start()
    {
        detectAttackStateForceExit = null;
        CanTriggerNextAttack = true;
        isTriggerAttack = false;
        IsAttack = false;
    }


    private void Update()
    {
        
    }
   

    #region 建立攻擊列表
    public void CreateAttackCollection()
    {
        SkillList.AttackParameter AttackParameter;

        AttackCollection = new Dictionary<int, SkillList.AttackParameter>();

        for (int i=0;i< skillList.normalAttack.Length; i++)
        {
            if (AttackCollection.TryGetValue(skillList.normalAttack[i].Id, out AttackParameter))
            {
                throw new System.Exception("Normal Attack Id已重複");

            }
            else
            {
                AttackCollection[skillList.normalAttack[i].Id] = skillList.normalAttack[i];
            }
         //   Debug.Log(skillList.normalAttack[i].Id);

        }

        for (int i = 0; i < skillList.specialAttack.Length; i++)
        {
            if (AttackCollection.TryGetValue(skillList.specialAttack[i].Id, out AttackParameter) )
            {
                throw new System.Exception("Special Attack Id已重複 specialAttack" +i+"Id = " + skillList.specialAttack[i].Id);

            }
            else
            {
                AttackCollection[skillList.specialAttack[i].Id] = skillList.specialAttack[i];

            }
        }

        for (int j = 0; j < skillList.deputyAttackCollections.Length; j++) 
        {
            for (int i = 0; i < skillList.deputyAttackCollections[j].DeputyAttack.Length; i++)
            {
                if (AttackCollection.TryGetValue(skillList.deputyAttackCollections[j].DeputyAttack[i].Id, out AttackParameter))
                {
                    throw new System.Exception("deputy Attack Collections "+ j + " DeputyAttack "+ i +"Id已重複 Id="+ skillList.deputyAttackCollections[j].DeputyAttack[i].Id);

                }
                else
                {
                    AttackCollection[skillList.deputyAttackCollections[j].DeputyAttack[i].Id] = skillList.deputyAttackCollections[j].DeputyAttack[i];

                }
            }
        }

       
    }
    #endregion

    public void JudgeInputKey(KeyCode input,string animatorTrigger)
    {
        



    }

    public void Attack(string animatorTrigger)
    {
        if (CanTriggerNextAttack)
        {
          //  Debug.Log("fff");
            if (detectAttackStateForceExit != null)
            {
                StopCoroutine(detectAttackStateForceExit);
            }

            StopCoroutine("resetTriggerAttack");
            StopCoroutine("DetectInput");
            animator.SetTrigger(animatorTrigger);

            CanTriggerNextAttack = false;
            isTriggerAttack = true;
            IsAttack = true;
        }

    }

   /* public void Attack()
    {
        if (currentAttackInfo.NextAttack.Length != 0)
        {
            for (int i = 0; i < currentAttackInfo.NextAttack.Length; i++)
            {
                if (Input.GetKeyDown(currentAttackInfo.NextAttack[i].keyCode))
                {
                    animator.SetTrigger(currentAttackInfo.NextAttack[i].AnimatorTriggerName);
                }

            }
        }       
        
    }*/

    #region 動畫事件
    public void GetAttackInfo(int Id)
    {
        currentAttackInfo = AttackCollection[Id];

        if (!currentAttackInfo.moveInfo.UseGravity)
        {
           // Debug.Log("Stop Use Gravity");
            gravity.StopUseGravity();
        }
    }

    public void StopTriggerNextAttack()
    {
        Debug.Log(isTriggerAttack);
        if (!isTriggerAttack)
        {
            CanTriggerNextAttack = false;

        }
    }

    public void TriggerNextAttack()
    {
     //   Debug.Log("jjjj");
        CanTriggerNextAttack = true;
        isTriggerAttack = false;

        StartCoroutine("DetectInput");
    }

    IEnumerator DetectInput()
    {        
        if (currentAttackInfo.NextAttack.Length != 0)
        {
            yield return new WaitUntil(() => DetectTriggerNextAttack());
        //    Debug.Log("TriggerNext");
        }
        else
        {
            yield return null;
        //    Debug.Log("dddd");
        }


    }

    private bool DetectTriggerNextAttack()
    {       
        if (currentAttackInfo.NextAttack != null)
        {
            for (int i = 0; i < currentAttackInfo.NextAttack.Length; i++)
            {

                if (Input.GetKeyDown(currentAttackInfo.NextAttack[i].keyCode))
                {
                    Attack(currentAttackInfo.NextAttack[i].AnimatorTriggerName);
                    return true;
                }              
            }            
        }
        return false;         
    }

    public void ResetTriggerAttack()
    {
       // Debug.Log(isTriggerAttack);
        if (!isTriggerAttack)
        {
            if (detectAttackStateForceExit != null)
            {
                StopCoroutine(detectAttackStateForceExit);
                Debug.Log("3. Reset Detect Attack State Force Exit");

            }
        }
        gravity.StartUseGravity();
        StopCoroutine("resetTriggerAttack");
        StartCoroutine("resetTriggerAttack");
    }

    IEnumerator resetTriggerAttack()
    {
        IsAttack = false;

        yield return new WaitForSeconds(0.5f);
        Debug.Log(IsAttack);

      /*  CanTriggerNextAttack = true;
         isTriggerAttack = false;*/
        //Debug.Log("Reset TriggerAttack");

        StopCoroutine("DetectInput");
       /* CanTriggerNextAttack = true;
        isTriggerAttack = false; */
       // Debug.Log("stopDetectInput");

    }

    public void DetectForceExitAttack(string animationTag)
    {
        if (detectAttackStateForceExit != null)
        {
            StopCoroutine(detectAttackStateForceExit);
        }


        detectAttackStateForceExit = DetectAttackStateForceExit(animationTag);

        StartCoroutine(detectAttackStateForceExit);
    }

    IEnumerator DetectAttackStateForceExit(string animationTag)
    {
        //Debug.Log("1.Detect Attack State Force Exit");

       // yield return new WaitUntil(() => animationHash.GetCurrentAnimationTag(animationTag));
       // Debug.Log(animationHash.GetCurrentAnimationTag(animationTag));

        yield return new WaitUntil(() => !animationHash.GetCurrentAnimationTag(animationTag));
        Debug.Log("2.  Attack is State Force Exit");
        CanTriggerNextAttack = true;
        isTriggerAttack = false;
        //Debug.Log("Reset TriggerAttack");

        IsAttack = false;
        StopCoroutine("DetectInput");
    }

    #endregion


}
