using Photon.Realtime;
using Quantum;
using System;
using UnityEngine.Scripting;


[Preserve]

public unsafe class BoundController : SystemMainThreadFilter<BoundController.Filter> {

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public BoundLifeTimeComponent* Bound;
    }

    public override void Update(Frame f, ref Filter filter) {
        filter.Bound->DestroyTime -= f.DeltaTime;
            
        if(filter.Bound->DestroyTime <= 0) 
            f.Destroy(filter.Entity);
    }
}
