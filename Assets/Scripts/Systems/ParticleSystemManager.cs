using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Systems
{
    public class ParticleSystemManager : MonoBehaviour
    {
        public static ParticleSystemManager Get { get; private set; }

        private void Awake()
        {
            if (Get)
            {
                Destroy(gameObject);
                return;
            }

            Get = this;
        }


        public List<ParticleEffect> particleEffects;

        public void Play(string id, bool instantiateMultipleParticleEffects = false)
        {
            if (particleEffects.Find(p => p.particleEffectId.Equals(id)) is { } found)
            {
                found.Play(this, transform.position, instantiateMultipleParticleEffects, -1);
            }
        }

        public void Play(string id, Vector3 particleEffectPosition, bool instantiateMultipleParticleEffects = false)
        {
            if (particleEffects.Find(p => p.particleEffectId.Equals(id)) is { } found)
            {
                found.Play(this, particleEffectPosition, instantiateMultipleParticleEffects, -1);
            }
        }

        public void Play(string id, Vector3 particleEffectPosition, float delayBeforeReset,
            bool instantiateMultipleParticleEffects = false)
        {
            if (particleEffects.Find(p => p.particleEffectId.Equals(id)) is { } found)
            {
                found.Play(this, particleEffectPosition, instantiateMultipleParticleEffects, delayBeforeReset);
            }
        }

        public void Stop(string id)
        {
            if (particleEffects.Find(p => p.particleEffectId.Equals(id)) is { } found)
            {
                found.Stop(this);
            }
        }
    }


    [Serializable]
    public class ParticleEffect
    {
        public string particleEffectId;
        public ParticleSystem particleEffectPrefab;
        private Coroutine m_PECoroutine;
        private ParticleSystem m_Clone;

        public void Play(ParticleSystemManager manager, Vector3 offsetPos, bool instantiateMultipleParticleEffects,
            float resetAfterTime)
        {
            m_Clone = instantiateMultipleParticleEffects && m_Clone
                ? m_Clone
                : DynamicInstantiateParticleEffect(particleEffectPrefab);
            m_Clone.gameObject.SetActive(true);
            if (m_Clone)
            {
                m_Clone.Play();
                if (m_PECoroutine != null)
                    manager.StopCoroutine(m_PECoroutine);
                manager.StartCoroutine(UpdateParticleEffectTransform(m_Clone, offsetPos, resetAfterTime));
            }
        }

        public void Stop(ParticleSystemManager manager)
        {
            if (m_PECoroutine != null)
                manager.StopCoroutine(m_PECoroutine);

            m_Clone.Stop();
            m_Clone.gameObject.SetActive(false);
        }


        private IEnumerator UpdateParticleEffectTransform(ParticleSystem clone, Vector3 offsetPos, float resetAfterTime)
        {
            float time = 0;

            while (true)
            {
                time += Time.deltaTime;
                clone.transform.position = offsetPos;
                yield return new WaitForEndOfFrame();

                if (time >= resetAfterTime && resetAfterTime > 0)
                {
                    clone.Stop();
                    clone.gameObject.SetActive(false);
                }
            }
        }


        private static Transform parent;

        private static Dictionary<int, List<ParticleSystem>> dictionaryOfPooledParticleSystems =
            new Dictionary<int, List<ParticleSystem>>();

        private static ParticleSystem DynamicInstantiateParticleEffect(ParticleSystem original)
        {
            bool couldntFindAvailableEffect =
                !dictionaryOfPooledParticleSystems.ContainsKey(original.GetInstanceID()) ||
                dictionaryOfPooledParticleSystems[original.GetInstanceID()].All(p => p.gameObject.activeSelf);

            if (couldntFindAvailableEffect)
            {
                parent = parent ? parent : GameObject.FindObjectOfType<ParticleSystemManager>().transform;
                List<ParticleSystem> pe = new List<ParticleSystem>();

                for (int i = 0; i < 100; i++)
                {
                    ParticleSystem clone = GameObject.Instantiate(original, parent);
                    clone.transform.localPosition = Vector3.zero;
                    clone.transform.localRotation = Quaternion.identity;
                    clone.gameObject.SetActive(false);
                    pe.Add(clone);
                }

                if (dictionaryOfPooledParticleSystems.ContainsKey(original.GetInstanceID()))
                {
                    dictionaryOfPooledParticleSystems[original.GetInstanceID()].AddRange(pe);
                }
                else
                {
                    dictionaryOfPooledParticleSystems.Add(original.GetInstanceID(), pe);
                }
            }

            if (dictionaryOfPooledParticleSystems.ContainsKey(original.GetInstanceID()))
            {
                foreach (var PE in dictionaryOfPooledParticleSystems[original.GetInstanceID()])
                {
                    if (!PE.gameObject.activeSelf)
                        return PE;
                }
            }

            return null;
        }
    }
}