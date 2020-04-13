using UnityEngine;
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
        private Animation _Animation = null; // todo: use Animator
        protected float range = 35.0f;

        [HideInInspector] public UnityEvent onHitEvent = new UnityEvent();

        public virtual void OnDropped() { }
        public virtual void OnFireInput() { }


        public bool IsBusy()
        {
            // todo: Should have animator on the weapon itself
            if (this._Animation == null)
                this._Animation = Camera.main.transform.GetComponent<Animation>();

            return this._Animation.isPlaying;
        }

        /*
        // todo: put in AIWeaponController?
        // (Optional) AI ammo count (doesn't have a seperate inventory).
        [SerializeField] private int AiAmmo = 300;
        // is AI? (decided automatically).
        private bool isAI = false;

        // todo: controller
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

            // Return if clip is full or not empty.
            if (!(remainingAmmo < magSize || remainingAmmo <= 0))
                return false;

            switch (gunType)
            {

                case GunType.AssaultRifle:

                    // If bullets still in clip, add it back to the ammo.
                    if (remainingAmmo > 0)
                        Inventory.instance.ManipulateAmmo(gunType, +remainingAmmo);

                    // If there is ammo, add it to the clip (even if can't fill).
                    if (Inventory.instance.GetAmmo(gunType) > 0)
                    {
                        if (Inventory.instance.GetAmmo(gunType) < magSize)
                        {
                            remainingAmmo = Inventory.instance.GetAmmo(gunType);
                            Inventory.instance.SetAmmo(gunType, 0);
                        }
                        else
                        {
                            remainingAmmo = magSize;
                            Inventory.instance.ManipulateAmmo(gunType, -magSize);
                        }

                        // Update clip UI Text element.
                        if (ammoText)
                            ammoText.text = remainingAmmo.ToString();

                    }
                    break;

                case GunType.Pistol:
                    Debug.Log("Weapon.cs/Reload(): TODO - Pistol Case");
                    break;

                case GunType.Shotgun:
                    Debug.Log("Weapon.cs/Reload(): TODO - Shotgun Case");
                    break;

                default:
                    Debug.Log("Weapon.cs/Reload(): TODO - GunType");
                    break;

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

        // Getter/Setter functions
         
        public Animation GetAnimation()
        {
            if (_Animation == null)
                Debug.LogError("Weapon.cs/GetAnimation(): _Animation variable == null");

            return _Animation;
        }

        public void SetAnimation(Animation anim)
        {
            _Animation = anim;
        }

        public GunType GetWeaponType()
        {
            return gunType;
        }


        public int GetClip()
        {
            return remainingAmmo;
        }

        public void ManipulateClip(int value)
        {
            remainingAmmo += value;

            // Update clip UI text element.
            if (ammoText)
                ammoText.text = remainingAmmo.ToString();
        }

        public void AIManipulateAmmo(int value)
        {
            AiAmmo += value;
            if (AiAmmo < 0)
                AiAmmo = 0;
        }

        public int AIGetAmmo()
        {
            return AiAmmo;
        }

        public FireType GetFireType()
        {
            return fireType;
        }

        public int GetDamage()
        {
            return damage;
        }

        public GameObject GetScope()
        {
            return scopeGO;
        }

        public Vector3 GetPickUpPosition()
        {
            return PickUpPos;
        }

        public bool GetUseRayBullet()
        {
            return useProjectiles;
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

        public bool GetPowerUpCapable()
        {
            return _PowerUpCapable;
        }
        */
    }
}