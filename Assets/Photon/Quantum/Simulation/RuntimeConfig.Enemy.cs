
namespace Quantum {
    //�̹� RuntimeConfig Ŭ������ �����ϱ� ������, �ش� Ŭ������ Ȯ���ϱ� ���� partial Ű���� ���
    public partial class RuntimeConfig {
        //QuantumRunner�� �ش� �ʵ尡 �߰��ȴ�.
        public AssetRef<EnemyGameConfig> EnemyGameConfig;
        public AssetRef<PlayerConfig> PlayerGameConfig;
        public AssetRef<PlayerProjectileConfig> PlayerProjectileGameConfig;

        public AssetRef<EnemyManagerConfig> EnemyManagerConfig;
    }
}