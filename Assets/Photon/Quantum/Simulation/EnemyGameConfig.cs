using Photon.Deterministic;
using Quantum;
using UnityEngine;

//���� ������Ʈ�� ��ӹ�����, �ùķ��̼ǿ��� �Ʒ��� �������� ����� �� �ְ� �ȴ�.
//���� ������Ʈ��, ��ũ���ͺ������Ʈ�� ��ӹް� �ֱ� ������ �����Ϳ��� �Ʒ��� �������� ������ �� �ְ� �ȴ�.
public class EnemyGameConfig : AssetObject {
    [Header("Enemys configuration")]
    [Tooltip("Prototype reference to spawn Enemys")]
    //����Ƽ�� ������ �ùķ��̼ǿ��� �����ϱ� ���� AssetRef�� ���
    public AssetRef<EntityPrototype> EnemyPrototype;
    [Tooltip("Speed applied to the Enemy when spawned")]
    public FP EnemyInitialSpeed = 8;
    [Tooltip("Minimum torque applied to the Enemy when spawned")]
    public FP EnemyInitialTorqueMin = 7;
    [Tooltip("Maximum torque applied to the Enemy when spawned")]
    public FP EnemyInitialTorqueMax = 20;
    [Tooltip("Distance to the center of the map. This value is the radius in a random circular location where the Enemy is spawned")]
    public FP EnemySpawnDistanceToCenter = 20;
    [Tooltip("Amount of Enemys spawned in level 1. In each level, the number os Enemys spawned is increased by one")]
    public int InitialEnemysCount = 5;
}
