using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AttackSystem : MonoBehaviour
{
    public SkillList skillList;
    private Animator animator;
    private Dictionary<int, SkillList.AttackParameter> AttackCollection;

    private bool CanTriggerNextAttack;
    private bool isTriggerAttack;
    private SkillList.AttackParameter currentAttackInfo;


    private void Awake()
    {
        CreateAttackCollection();
        animator = GetComponent<Animator>();
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

    public void NormalAttack(string animatorTrigger)
    {
        if (CanTriggerNextAttack)
        {
            animator.SetTrigger(animatorTrigger);

            CanTriggerNextAttack = false;
            isTriggerAttack = true;

        }

    }


    public void Attack()
    {
        if (currentAttackInfo.deputyAttack.Length != 0)
        {
            for (int i = 0; i < currentAttackInfo.deputyAttack.Length; i++)
            {
                if (Input.GetKeyDown(currentAttackInfo.deputyAttack[i].keyCode))
                {
                    animator.SetTrigger(currentAttackInfo.deputyAttack[i].AnimatorTriggerName);
                }

            }
        }       

    }

    #region 動畫事件
    public void GetAttackInfo(int Id)
    {
        currentAttackInfo = AttackCollection[Id];
    }

    public void TriggerNextAttack()
    {
        CanTriggerNextAttack = true;
        isTriggerAttack = false;

    }

    public void DoNotTriggerAttack()
    {
        CanTriggerNextAttack = false;
    }

    #endregion


}
