using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
    [System.Serializable]
    public struct MoveParameter
    {
        public float RunSpeed;
        public float FastRunSpeed;
        public float RotateSpeed;
    }
    [System.Serializable]
    public struct JumpParameter
    {
        public float JumpHigh;
        public float DoubleJumpHigh;
        public float JumpMoveSpeed;      
    }

    [System.Serializable]
    public struct AvoidParameter
    {
        public float AvoidSpeed;
        public float AvoidDistance;
    }
    [System.Serializable]
    public struct DamageParameter
    {
        public float GetupTime;
        public AudioClip AudioClip_Damage;
    }
    [System.Serializable]
    public struct DieParameter
    {
        public AudioClip AudioClip_Damage;
    }
    [System.Serializable]
    public struct AttackParameter
    {
        public int Combo;
        public float AttackValue;
        public AudioClip AudioClip_Attack;
        public GameObject Particle_Attack;
        public Vector3 ParticleDistanceFormPlayer;
    }
    [System.Serializable]
    public class PlayerParameter
    {
        public MoveParameter moveParameter;
        public JumpParameter jumpParameter;
        public AvoidParameter avoidParameter;
        public DamageParameter damageParameter;
        public DieParameter dieParameter;
        public AttackParameter[] normalAttack;
        public AttackParameter[] specialAttack;
        public AttackParameter[] changeAttack;

    }

    public PlayerParameter playerParameter;
    public GameObject Player;
    public GameObject MainCamera;
}
