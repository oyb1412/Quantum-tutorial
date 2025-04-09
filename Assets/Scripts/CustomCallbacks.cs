using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCallbacks : QuantumCallbacks
{
    public override void OnGameStart(QuantumGame game) {
        Debug.Log("gogo");
    }
}
