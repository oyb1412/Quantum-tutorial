using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.Windows;

public class GameInput : MonoBehaviour
{
    private void OnEnable() {
        QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
    }

    public void PollInput(CallbackPollInput callback) {
        Quantum.Input i = new Quantum.Input();

        i.Left = UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow);
        i.Right = UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow);
        i.Up = UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow);
        i.Down = UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow);
        i.Attack = UnityEngine.Input.GetKey(KeyCode.Space);

        Debug.Log($"입력 요청된 슬롯 : {callback.PlayerSlot}");
        callback.SetInput(i, DeterministicInputFlags.Repeatable);
    }
}
