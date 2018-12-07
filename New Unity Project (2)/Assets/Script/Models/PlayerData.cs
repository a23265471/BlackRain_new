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
    public struct PlayerParameter
    {
        public MoveParameter moveParameter;
        public AvoidParameter avoidParameter;
        public DamageParameter damageParameter;
        public DieParameter dieParameter;
        public AttackParameter[] normalAttack;
        public AttackParameter[] specialAttack;
        public AttackParameter[] changeAttack;

    }

    public PlayerParameter playerParameter;
}
