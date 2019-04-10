using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class SkillList : ScriptableObject
{
    [System.Serializable]
    public struct AttackParameter
    {
        public string Name;
        public int Id;
        public int AttackOder;
        public ChangeToDeputyAttack[] deputyAttack;
        public float MoveSpeed;
        public float MoveDistance;
        public int MoveDirection_X;
        public int MoveDirection_Y;
        public int MoveDirection_Z;
        public bool UseGravity;

        public AudioClip AudioClip_Attack;
        // public GameObject Particle_Attack;    
    }

    [System.Serializable]
    public struct DeputyAttackCollection
    {
        public AttackParameter[] DeputyAttack;
    }

    [System.Serializable]
    public struct ChangeToDeputyAttack
    {
        public string Id;
        public KeyCode keyCode;
        public string AnimatorTriggerName;

    }

    public AttackParameter[] normalAttack;  
    public DeputyAttackCollection[] deputyAttackCollections;
    public AttackParameter[] specialAttack;
}
