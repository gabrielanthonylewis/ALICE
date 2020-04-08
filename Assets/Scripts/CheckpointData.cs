﻿using UnityEngine;

namespace ALICE.Checkpoint
{
    [System.Serializable]
    public class CheckpointData
    {
        public Vector3[] enemyPositions = new Vector3[0];
        public Vector3 playerPosition;
        public Quaternion playerRotation;
        public int ammo;
        public int grenades;
        public float health;
        // todo guns
    };
}
