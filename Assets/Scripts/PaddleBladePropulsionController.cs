using System;
using UnityEngine;

namespace KayakingVR
{
    public class PaddleBladePropulsionController: MonoBehaviour
    {        
        [SerializeField] 
        private Collider waterTrigger;

        [SerializeField] 
        private float dragForce = 50f;

        [SerializeField] 
        private Rigidbody kayakBody;

        private Vector3 movementCounter = Vector3.zero;
        private Vector3 prevPosition;

        private void Awake()
        {
            prevPosition = transform.position;
        }

        private void Update()
        {
            var currentPosition = transform.position;
            movementCounter += currentPosition - prevPosition;
            prevPosition = currentPosition;

        }

        private void OnTriggerEnter(Collider other)
        {
            if(other!=waterTrigger)
                return;
            
            movementCounter = Vector3.zero;
        }

        private Vector3 GetWaterLevelPosition()
        {
            var contactPos = transform.position;
            contactPos.y = waterTrigger.transform.position.y;
            return contactPos;
        }

        private void OnTriggerStay(Collider other)
        {
            if(other!=waterTrigger)
                return;
            
            ApplyForce(movementCounter);
            movementCounter = Vector3.zero; // reset the counter
        }

        private void ApplyForce(Vector3 movement)
        {
            var propulsionForce = movement * dragForce;
            
            // invert force to achieve - blade moves left, kayak moves right
            propulsionForce.x = -1f * propulsionForce.x;
            propulsionForce.z = -1f * propulsionForce.z;
            propulsionForce.y = 0; // ignore up vector, because the kayak moves in a plane space
            
            Debug.Log($"Force: {propulsionForce}");
            kayakBody.AddForceAtPosition(propulsionForce, GetWaterLevelPosition());
        }
        
        private void OnTriggerExit(Collider other)
        {            
            if(other!=waterTrigger)
                return;

        }
    }
}