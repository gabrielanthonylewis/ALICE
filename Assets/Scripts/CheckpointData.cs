using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public class CheckpointData
    {
        public Vector3[] enemyPositions = new Vector3[0];
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public int ammo; // todo: Bug: Stores 240 (+30 in clip), then when load Weapon takes 30 so ends up as 210..
        // so could add all gun's ammo to the Ammo when call "GetTOTALAmmo()", GOOD IDEA
        public int grenades;
        // todo health
        // todo guns
    };
}
