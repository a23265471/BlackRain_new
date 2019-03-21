using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    [System.Serializable]
    public struct ParticleTransform
    {
        public string Id;
        public Transform ParticlePos;
    }    
    public ParticleList ParticleData;   
    public ParticleTransform[] particleTransform;

    private Dictionary<string, ParticleSystem> ParticleCollection;

    private void Awake()
    {
        CreateParticleCollection();
    }


    private void CreateParticleCollection()
    {
        ParticleCollection = new Dictionary<string, ParticleSystem>();

        ParticleSystem particleSystem;
        if(ParticleData.particlesCollection.Length!= particleTransform.Length)
        {
            throw new System.Exception("Particle數與位置數不符");
        }
        else
        {
            for (int j = 0; j < ParticleData.particlesCollection.Length; j++)
            {
                for (int i = 0; i < particleTransform.Length; i++)
                {
                    if (particleTransform[i].Id == ParticleData.particlesCollection[j].Id)
                    {
                        if(ParticleCollection.TryGetValue(particleTransform[i].Id,out particleSystem))
                        {
                            throw new System.Exception("Id已重複");
                        }
                        else
                        {
                          //  Debug.Log("dd");
                            if(ParticleData.particlesCollection[j].Particle!=null&& particleTransform[i].ParticlePos != null)
                            {
                                GameObject particle = Instantiate(ParticleData.particlesCollection[j].Particle, particleTransform[i].ParticlePos.position, particleTransform[i].ParticlePos.transform.rotation, transform);
                                ParticleCollection[particleTransform[i].Id] = particle.GetComponent<ParticleSystem>();

                            }
                            else
                            {
                                throw new System.Exception("Object is null");
                            }

                        }

                    }
                }

            }
        }
        
    }

    public ParticleSystem GetParticle(string Id)
    {
        ParticleSystem particleSystem;

        if (ParticleCollection.TryGetValue(Id, out particleSystem))
        {
            return particleSystem;

        }
        else
        {
            throw new System.Exception("Particle is not Find");
          //  return null;
        }


        
    }


}
