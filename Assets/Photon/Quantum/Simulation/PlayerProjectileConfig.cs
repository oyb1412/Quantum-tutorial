using Photon.Deterministic;
using Quantum;
using UnityEngine;

public class PlayerProjectileConfig : AssetObject {
    [Tooltip("Speed applied to the projectile when spawned")]
    public FP ProjectileInitialSpeed = 15;

    [Tooltip("Time until destroy the projectile")]
    public FP ProjectileTTL = 1;
}
