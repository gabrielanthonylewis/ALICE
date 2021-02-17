using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public class CheckpointData
    {
        public ActorData[] enemies = new ActorData[0];
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

    [System.Serializable]
    public class ActorData
    {
        public string objectID;
        public Vector3 position;
        public Quaternion rotation;
    }
}
