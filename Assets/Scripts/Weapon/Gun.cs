using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Gun : Weapon
{
    private enum FireType
    {
        Auto = 0,
        Semi = 1,
        Single = 2
    };

    [SerializeField] private int magSize = 30;
    [SerializeField] private ParticleSystem muzzleFlashPS = null;
    [SerializeField] private Text ammoText = null;
    [SerializeField] private FireType fireType = FireType.Auto;
    [SerializeField] private bool useProjectiles = true;
    [SerializeField] private AudioClip fireSound = null;
    [SerializeField] private AudioClip reloadSound = null;
    [SerializeField] protected AudioClip fireTypeSound;
    [SerializeField] private int semiFireBulletCount = 4;
    [SerializeField] private float bulletForce = 1.0f;
    [SerializeField] private GameObject bulletHolePrefab = null;
    [SerializeField] private float fireDelay = 0.1f;
    [SerializeField] protected Vector2 hipFireOffsetRange = new Vector2(-0.05f, 0.05f);
    [SerializeField] protected float hipFireOffsetMultiplier = 1.0f;

    private int remainingMagAmmo = 0;
    protected bool isFiring = false;
    
    private void Start()
    {
        // Current clip should be fully loaded.
        this.SetRemainingAmmo(this.magSize);
    }

    public override void OnFireInput(bool isDownOnce)
    {
        base.OnFireInput(isDownOnce);

        if(this.CanFire(isDownOnce))
            this.StartCoroutine(this.Fire());
    }

    private IEnumerator Fire()
    {
        this.isFiring = true;

        int bulletsToShoot = (this.fireType == FireType.Semi) ? this.semiFireBulletCount : 1;
        for(int i = 0; i < bulletsToShoot; i++)
        {
            if(this.remainingMagAmmo <= 0)
                break;

            this.FireBullet(this.GetFireVector(),
                this.GetFireRayPosition(), this.GetFireForwardVector());

            yield return new WaitForSeconds(this.fireDelay);
        }

        // Auto reload
        if(this.remainingMagAmmo <= 0)
            this.OnReloadInput();

        this.isFiring = false;
    }

    public virtual void FireBullet(Vector3 randomVector, Vector3 rayPos, Vector3 forward)
    {
        this.animator.SetTrigger("shotFired");
        this.EnableMuzzleFlash(true);
        this.audioSource.PlayOneShot(this.fireSound);

        // If the weapon has a projectile then fire it.
        if (this.transform.GetComponent<FireObject>())
            this.transform.GetComponent<FireObject>().Fire();
        else
        {
            /* Shoot a bullet via raycasting.
             * ~0 means every layer, also triggers are ignored. */
            RaycastHit hit;
            if (Physics.Raycast(rayPos, forward + randomVector, out hit, this.range, ~0, QueryTriggerInteraction.Ignore))
            {
                bool hasHitObject = false;

                // Bullet hole.
                if(this.bulletHolePrefab != null)
                    GameObject.Instantiate(this.bulletHolePrefab, hit.point - (transform.forward * 0.02f), Quaternion.Euler(hit.normal));

                // Powerup.
                if(this.IsPowerupActive())
                {
                    if(this.GetActivePowerup().TryAffectObject(hit.transform))
                        hasHitObject = true;
                }

                // Forward force to the hit object (that isn't a character).
                if (hit.transform.tag != "Enemy" && hit.transform.tag != "Player")
                {
                    Rigidbody hitRigidbody = hit.transform.GetComponent<Rigidbody>();
                    if(hitRigidbody != null)
                    {
                        hitRigidbody.AddForce(this.transform.forward * this.bulletForce);
                        hasHitObject = true;
                    }
                }

                // Damage.
                Destructable hitDestructable = hit.transform.GetComponent<Destructable>();
                if(hitDestructable != null)
                {
                    hit.transform.GetComponent<Destructable>().ManipulateHealth(-this.damage);
                    hasHitObject = true;
                }

                if(hasHitObject)
                    this.onHitEvent?.Invoke();
            }
        }

        this.SetRemainingAmmo(this.remainingMagAmmo - 1);
    }

    public void NextFireType()
    {
        this.fireType = (FireType)Mathf.Repeat((int)this.fireType + 1,
            System.Enum.GetValues(typeof(FireType)).Length);

        this.animator.SetInteger("fireType", (int)this.fireType);
    }

    public override void OnReloadInput()
    {
        base.OnReloadInput();

        bool isMagFull = (this.remainingMagAmmo == this.magSize);
        bool canReload = (!isMagFull && this.weaponController.inventory.GetAmmo() > 0);
        if(!canReload)
            return;

        // If bullets still in clip, add it back to the Inventory's ammo.
        if (this.remainingMagAmmo > 0)
            this.weaponController.inventory.ManipulateAmmo(this.remainingMagAmmo);

        // If there is ammo, add it to the clip (even if can't fill).
        int newAmmo = this.weaponController.inventory.TryTakeAmmo(this.magSize);
        if(newAmmo > 0)
        {
            this.StopCoroutine(this.Fire());

            this.animator.SetTrigger("reload");
            this.audioSource.PlayOneShot(this.reloadSound);

            this.SetRemainingAmmo(newAmmo);
        }
    }

    private void SetRemainingAmmo(int ammo)
    {
        this.remainingMagAmmo = ammo;

        if (this.ammoText)
            this.ammoText.text = this.remainingMagAmmo.ToString();
    }

    private void EnableMuzzleFlash(bool enable)
    {
        if(this.muzzleFlashPS == null)
            return;
            
        if (enable)
            this.muzzleFlashPS.Play();
        else
            this.muzzleFlashPS.Stop();

        this.muzzleFlashPS.gameObject.SetActive(enable);
    }

    public override void StopAllActivity()
    {
        base.StopAllActivity();

        this.StopAllCoroutines();
        this.EnableMuzzleFlash(false);
        this.isFiring = false;
    }

    private bool CanFire(bool isDownOnce)
    {
        // If not a single press then return during Semi/Single fire.
        if(!isDownOnce && this.fireType == FireType.Semi ||
            !isDownOnce && this.fireType == FireType.Single)
        {
            return false;
        }

        if (this.isFiring || this.IsReloading())     
            return false;

        return (this.remainingMagAmmo > 0);
    }

    private bool IsReloading()
    {
        return (this.animator.GetCurrentAnimatorStateInfo(0).IsName("reload") ||
                    this.animator.GetCurrentAnimatorStateInfo(0).IsName("reloadads"));
    }

    protected virtual Vector3 GetFireForwardVector()
    { 
        return Camera.main.transform.forward;
    }

    protected virtual Vector3 GetFireRayPosition() 
    {
        return Camera.main.transform.position;
    }

    protected virtual Vector3 GetFireVector()
    { 
        return (this.isAiming) ? Vector3.zero :
            this.GetRandomFireVector(this.hipFireOffsetRange, this.hipFireOffsetMultiplier); 
    }

    protected Vector3 GetRandomFireVector(Vector2 range, float multiplier)
    {
        return new Vector3(Random.Range(range.x, range.y), Random.Range(range.x, range.y),
            Random.Range(range.x, range.y)) * multiplier;
    }
}
