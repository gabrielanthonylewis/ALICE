using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public class CheckpointData
    {
        public int index = -1;
        public Vector3[] enemyPositions;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        //todo ammo
        // todo health
        // todo grenades
        // todo guns
    };
}
