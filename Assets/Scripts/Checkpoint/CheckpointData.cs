using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public class CheckpointData
    {
        public Vector3[] enemyPositions = new Vector3[0];
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public float health;
        public float slowmo;
        public InventoryData inventory;
    };

    [System.Serializable]
    public class InventoryData
    {
        public int ammo;
        public int grenades;
 		public string[] weaponNames;

        public InventoryData(int ammo,
            int grenades, string[] weaponNames)
        {
            this.ammo = ammo;
            this.grenades = grenades;
            this.weaponNames = weaponNames;
        }
    };
}
