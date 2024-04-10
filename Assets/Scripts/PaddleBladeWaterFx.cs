using KayakingVR.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace KayakingVR
{
    public class PaddleBladeWaterFx: MonoBehaviour
    {
        [SerializeField] 
        private Collider waterTrigger;
        
        [SerializeField] 
        private ParticleSystem splashFx;
        
        [SerializeField] 
        private AudioSource splashAudioFx;
        
        [SerializeField]
        [Tooltip("Relation between hit force of the blade on the water and size of the splash fx")]
        public AnimationCurve splashForce =  AnimationCurve.Linear(0, 0, 10, 1);

        [SerializeField] 
        private ParticleSystem foamFx;

        private LowPassFilter lowPassFilter;
        private float velocity3dFiltered;
        private Vector3 prevPosition;
        private Transform mTransform;
        private void Awake()
        {
            mTransform = transform;
            prevPosition = mTransform.position;
            
            if(!waterTrigger.isTrigger)
                Debug.LogError("Water collider must be trigger");
            
            foamFx.Stop();
        }

        private void Update()
        {
            // calculate velocity of the blade
            var movement = Vector3.Distance(mTransform.position, prevPosition);
            var velocity3dRaw = movement / Time.deltaTime;
            
            // to avoid issues when the movement changes drastically, smooth it
            velocity3dFiltered = Mathf.Lerp(velocity3dFiltered, velocity3dRaw, 0.3f);
            
            prevPosition = mTransform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other!=waterTrigger)
                return;

            OnTriggerStay(other);   //update position of the effects

            foamFx.Play();
            var size = splashForce.Evaluate(velocity3dFiltered);
            
            // calculate common parameters for tha particles
            var particle = new ParticleSystem.EmitParams()
            {
                position = GetWaterLevelPosition(),
            };
            var particleSize = Mathf.Lerp(splashFx.main.startSize.constantMin, splashFx.main.startSize.constantMax,
                size);
            
            // emit 5 particles with some randomness to them
            for (int i = 0; i < 6; i++) 
            {
                particle.startSize = particleSize * Random.Range(0.5f, 1.5f);
                particle.rotation3D = Random.onUnitSphere * 45f;
                splashFx.Emit(particle,1);
            }
                
            splashAudioFx.PlayOneShot(splashAudioFx.clip, size);
            Debug.Log($"size: {size}; velocity: {velocity3dFiltered};");
        }

        private Vector3 GetWaterLevelPosition()
        {
            var contactPos = mTransform.position;
            contactPos.y = waterTrigger.transform.position.y;
            return contactPos;
        }

        private void OnTriggerStay(Collider other)
        {
            if(other!=waterTrigger)
                return;

            foamFx.transform.position = GetWaterLevelPosition();
        }

        private void OnTriggerExit(Collider other)
        {            
            if(other!=waterTrigger)
                return;

            foamFx.Stop();
        }
    }
}