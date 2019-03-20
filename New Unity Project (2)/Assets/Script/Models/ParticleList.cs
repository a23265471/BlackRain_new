using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ParticleList : ScriptableObject
{
    [System.Serializable]
    public struct Particles
    {
        public string Id;
        public GameObject Particle;
    }
    
    public Particles[] particlesCollection;
    

}
