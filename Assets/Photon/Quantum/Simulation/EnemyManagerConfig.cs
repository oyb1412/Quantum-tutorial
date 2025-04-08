using Photon.Deterministic;
using Quantum;
using UnityEngine;

//에셋 오브젝트를 상속받으면, 시뮬레이션에서 아래의 변수들을 사용할 수 있게 된다.
//에셋 오브젝트는, 스크립터블오브젝트를 상속받고 있기 때문에 에디터에서 아래의 변수들을 편집할 수 있게 된다.
public class EnemyManagerConfig : AssetObject {
    [Tooltip("Prototype reference to spawn Enemys")]
    //유니티의 에셋을 시뮬레이션에서 참조하기 위해 AssetRef를 사용
    public AssetRef<EntityPrototype> EnemyPrototype;
}