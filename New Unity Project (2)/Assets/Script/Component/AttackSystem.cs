using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackSystem : MonoBehaviour
{
    public SkillList skillList;
    private Animator animator;
    public Dictionary<int, SkillList.AttackParameter> AttackCollection;

    private bool CanTriggerNextAttack;
    public bool isTriggerAttack;
    private SkillList.AttackParameter currentAttackInfo;

    public bool IsAttack;

    private void Awake()
    {
        CreateAttackCollection();
        animator = GetComponent<Animator>();
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
    }

    void Start()
    {
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
            StopCoroutine("resetTriggerAttack");

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
    }

    public void TriggerNextAttack()
    {
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
        StopCoroutine("resetTriggerAttack");
        StartCoroutine("resetTriggerAttack");
    }

     IEnumerator resetTriggerAttack()
     {

         yield return new WaitForSeconds(0.2f);
         
             CanTriggerNextAttack = true;
             isTriggerAttack = false;
            //Debug.Log("Reset TriggerAttack");
        
        IsAttack = false;
        StopCoroutine("DetectInput");
       // Debug.Log("stopDetectInput");

    }


    /*   IEnumerator resetToIdle()
       {
           yield return new WaitUntil(() => );


       }*/

    #endregion


}
