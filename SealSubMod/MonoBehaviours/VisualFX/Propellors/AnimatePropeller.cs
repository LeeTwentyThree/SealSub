﻿namespace SealSubMod.MonoBehaviours.VisualFX.Propellors;

internal class AnimatePropeller : MonoBehaviour
{
    [SerializeField] PropellerManager manager;
    [SerializeField] Vector3 rotationAxis;

    private void Update()
    {
        transform.localEulerAngles += rotationAxis * manager.RotationsPerSecond * Time.deltaTime;
    }
}
