﻿using UnityEngine;
using System.Collections;

namespace MalbersAnimations
{
    public class FlyBehavior : StateMachineBehaviour
    {

        public float DownAcceleration = 4;
        public float FallRecovery = 1.5f;

        float acceleration = 0;
        Rigidbody rb;
        Animal animal;

        float time;

        Vector3 v;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            rb = animator.GetComponent<Rigidbody>();
            animal = animator.GetComponent<Animal>();
            acceleration = 0;

            if (rb)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;

                v = animator.GetCurrentAnimatorStateInfo(layerIndex).tagHash == Hash.Fall ? v = rb.velocity : Vector3.zero; //Just recover if your coming from the fall animations

                Vector3 cleanY = new Vector3(rb.velocity.x, 0, rb.velocity.z); //Clean the Y velocity
                rb.velocity = cleanY;

                rb.drag = 0;
                rb.useGravity = false;
            }

            animator.applyRootMotion = true;
        }

        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
           

            if (v != Vector3.zero) //Means that last animation was falling 
            {
                //This is for changing from fall to Fly
                animator.applyRootMotion = true;
                animator.transform.position = Vector3.Lerp(animal.transform.position, animal.transform.position + v, time);
                v = Vector3.Lerp(v, Vector3.zero, time * FallRecovery);
            }

            //Add more speed when going Down

            time = animator.updateMode == AnimatorUpdateMode.AnimatePhysics ? Time.fixedDeltaTime : Time.deltaTime;

            if (animator.updateMode == AnimatorUpdateMode.Normal) return;           //Hack to skip while is in normal Update mode

            if (animal.MovementAxis.y < -0.1)
            {
                acceleration = Mathf.Lerp(acceleration, acceleration + DownAcceleration, time);
            }
            else
            {
                float a = acceleration - DownAcceleration;
                if (a < 0) a = 0;

                acceleration = Mathf.Lerp(acceleration, a, time);  //Deacelerate slowly all the acceleration you earned..
            }

            animator.transform.position = Vector3.Lerp(animator.transform.position, animator.transform.position + animator.velocity * (acceleration / 2), time);

           
        }
    }
}