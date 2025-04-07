using Photon.Deterministic;
using Quantum;
using UnityEngine;

//���� ������Ʈ�� ��ӹ�����, �ùķ��̼ǿ��� �Ʒ��� �������� ����� �� �ְ� �ȴ�.
//���� ������Ʈ��, ��ũ���ͺ������Ʈ�� ��ӹް� �ֱ� ������ �����Ϳ��� �Ʒ��� �������� ������ �� �ְ� �ȴ�.
public class EnemyManagerConfig : AssetObject {
    [Tooltip("Prototype reference to spawn Enemys")]
    //����Ƽ�� ������ �ùķ��̼ǿ��� �����ϱ� ���� AssetRef�� ���
    public AssetRef<EntityPrototype> EnemyPrototype;
    public FP EnemyMoveSpeed = 8;
    public int CurrentEnemyHp = 10;
    public int MaxEnemyHp = 10;
    public FP EnemyAttackSpeed = 1;
    public FP EnemyAttackTimer = 0;
    public FP EnemySpawnDistanceToCenter = 20;

}