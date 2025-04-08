
namespace Quantum {
    //이미 RuntimeConfig 클래스가 존재하기 때문에, 해당 클래스를 확장하기 위해 partial 키워드 사용
    public partial class RuntimeConfig {
        //QuantumRunner에 해당 필드가 추가된다.
        public AssetRef<EnemyGameConfig> EnemyGameConfig;
        public AssetRef<PlayerConfig> PlayerGameConfig;
        public AssetRef<PlayerProjectileConfig> PlayerProjectileGameConfig;

        public AssetRef<EnemyManagerConfig> EnemyManagerConfig;
    }
}