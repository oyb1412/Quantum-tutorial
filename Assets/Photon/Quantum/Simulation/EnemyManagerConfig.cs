using Photon.Deterministic;
using Quantum;
using UnityEngine;

//���� ������Ʈ�� ��ӹ�����, �ùķ��̼ǿ��� �Ʒ��� �������� ����� �� �ְ� �ȴ�.
//���� ������Ʈ��, ��ũ���ͺ������Ʈ�� ��ӹް� �ֱ� ������ �����Ϳ��� �Ʒ��� �������� ������ �� �ְ� �ȴ�.
public class EnemyManagerConfig : AssetObject {
    [Tooltip("Prototype reference to spawn Enemys")]
    //����Ƽ�� ������ �ùķ��̼ǿ��� �����ϱ� ���� AssetRef�� ���
    public AssetRef<EntityPrototype> EnemyPrototype;
}