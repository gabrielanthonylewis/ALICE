using UnityEngine;

namespace ALICE.Weapon.Gun
{
    public class AssaultRifle : Gun
    {
        // Change the Fire Type (Fully Automatic, Burst and Single shot).
        public override void OnChangeFireTypeInput()
        {
            base.OnChangeFireTypeInput();

            if(this.isFiring)
                return;
            
            this.audioSource.PlayOneShot(this.fireTypeSound);
            this.NextFireType();
        }

        protected override Vector3 GetFireVector()
        {
            return (this.isAiming) ? Vector3.zero :
                this.GetRandomFireVector(this.hipFireOffsetRange, this.hipFireOffsetMultiplier);
        }

        protected override Vector3 GetFireForwardVector()
        {
            return Camera.main.transform.forward;
        }

        protected override Vector3 GetFireRayPosition()
        {
            return Camera.main.transform.position;
        }
    }
}
