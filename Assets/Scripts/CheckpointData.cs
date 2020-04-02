using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public struct CheckpointData
    {
        public int index;
        public Vector3[] enemyPositions;
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        //todo ammo
        // todo health
        // todo grenades
        // todo guns
    };
}
