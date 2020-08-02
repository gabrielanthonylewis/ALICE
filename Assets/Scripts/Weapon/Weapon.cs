﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

namespace ALICE.Weapon
{
    // The Weapon script contains weapon specific funtionallity such as reloading and changing the fire type.
    // It also stores the information such as the damage of the bullets and the current bullets in the clip.
    public class Weapon: MonoBehaviour
    {
        [SerializeField] protected int damage = 1;
        protected Animator animator = null;
        protected float range = 35.0f;

        [HideInInspector] public UnityEvent onHitEvent = new UnityEvent();

        public virtual void OnDropped() { }
        public virtual void OnFireInput() { }
        public virtual void OnReloadInput() { }
        public virtual void OnAimInput() { }

        private void Awake()
        {
            this.animator = this.GetComponent<Animator>();    
        }
        
        public bool IsBusy()
        {
            if (this.animator == null)
                return false;

            return (this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1 
                || this.animator.IsInTransition(0));
        }

        /*
        // TODO: put in AIWeaponController?
        // (Optional) AI ammo count (doesn't have a seperate inventory).
        [SerializeField] private int AiAmmo = 300;
        // is AI? (decided automatically).
        private bool isAI = false;

        // TODO: controller
        // Dependent on whether the Reload Coroutine is being run.
        private bool reloadRou = false;

        void Start()
        {

            // Automatically decide if weapon is owned by an AI.
            if (this.transform.parent == null)
                isAI = false;
            else if (this.transform.parent.GetComponent<AIWeaponController>())
                isAI = true;
            else
                isAI = false;
               
            // Manipulate ammo (because we initially load the clip).
            if (isAI)
                AIManipulateAmmo(-magSize);
            else
                Inventory.instance.ManipulateAmmo(gunType, -magSize);
        }

        // Change fire type to the next one, reseting after single shot.
        public void NextFireType()
        {
            // Play appropriate _Animation correseponding to the current Fire Type
            if ((int)fireType == 1)
            {
                this.GetAnimation()["FireRateToSingle"].speed = -1;
                this.GetAnimation().Play("FireRateToSingle");
                this.GetAnimation()["fireRateToSemi"].speed = -1;
                this.GetAnimation().Play("fireRateToSemi");
                fireType = (FireType)((int)fireType + 1);
            }
            else if ((int)fireType == 2)
            {
                this.GetAnimation()["fireRateToSemi"].speed = 1;
                this.GetAnimation().Play("fireRateToSemi");
                fireType = (FireType)((int)fireType + 1);
            }
            else if ((int)fireType == 3)
            {
                this.GetAnimation()["FireRateToSingle"].speed = 1;
                this.GetAnimation().Play("FireRateToSingle");
                fireType = (FireType)((int)fireType - 2);
            }

        }

        public bool Reload()
        {
            // Play Reload animtion.
            if (isAI)
            {

                if (!reloadRou)
                    StartCoroutine("ReloadRou");

                return true;
            }

            return true;
        }

        IEnumerator ReloadRou()
        {
            reloadRou = true;

            // Deactivate muzzle flash.
            muzzleFlashGO.SetActive(false);

            // Play Reload _Animation and wait for it to be complete.
            _Animation.Play("reload");
            yield return new WaitForSeconds(_Animation["reload"].length);

            // Update clip and AI Ammo (taking into account the case where AI ammo cannot fill the clip).
            if (AiAmmo < magSize)
            {
                remainingAmmo = AiAmmo;
                AiAmmo = 0;
            }
            else
            {
                remainingAmmo = magSize;
                AiAmmo -= magSize;
            }

            reloadRou = false;
        }

        public void AIManipulateAmmo(int value)
        {
            AiAmmo += value;
            if (AiAmmo < 0)
                AiAmmo = 0;
        }

        public void SwitchPowerUp()
        {
            if (!_PowerUpCapable)
                return;

            shrinkPS.gameObject.SetActive(false);
            transparencyPS.gameObject.SetActive(false);

            if (powerup == PowerUp.NULL)
            {
                powerup = PowerUp.Shrink;
                shrinkPS.gameObject.SetActive(true);
                return;
            }

            if (powerup == PowerUp.Shrink)
            {
                powerup = PowerUp.Transparency;
                transparencyPS.gameObject.SetActive(true);
                return;
            }

            if (powerup == PowerUp.Transparency)
            {
                powerup = PowerUp.NULL;
                return;
            }

        }
              
        private bool Shrink(Transform target, float val)
        {
            if (target.tag != "Resizable")
                return false;

            if (target.GetComponent<Transform>().localScale.x < 0.2f
                && target.GetComponent<Transform>().localScale.y < 0.2f
                && target.GetComponent<Transform>().localScale.z < 0.2f
               )
            {
                // Play power up sound to indicate no more can be done.
                if (powerupSound)
                {
                    if (Camera.main.GetComponent<AudioSource>())
                    {
                        Camera.main.GetComponent<AudioSource>().clip = powerupSound;
                        Camera.main.GetComponent<AudioSource>().Play();
                    }
                }
                return false;
            }

            // Downscale object.
            target.GetComponent<Transform>().localScale /= 1.1f;

            return true;
        }

        private bool ReduceAlpha(Transform target, float val)
        {
            if (target.tag != "Transparent")
                return false;


            Color tempCol = target.GetComponent<MeshRenderer>().material.color;
            tempCol.a -= val;

            // Disable the collider.
            if (tempCol.a <= 0.4f)
            {
                target.GetComponent<Collider>().enabled = false;
                // Play power up sound to indicate no more can be done.
                if (powerupSound)
                {
                    if (Camera.main.GetComponent<AudioSource>())
                    {
                        Camera.main.GetComponent<AudioSource>().clip = powerupSound;
                        Camera.main.GetComponent<AudioSource>().Play();
                    }
                }
                return false;
            }

            target.GetComponent<MeshRenderer>().material.color = tempCol;

            return true;
        }
        */
    }
}