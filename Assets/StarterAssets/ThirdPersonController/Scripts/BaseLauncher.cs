using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseLauncher : MonoBehaviour
{
    public float speed = 5f;
    public abstract void FireProjectile(GameObject head, GameObject target, Vector3 Force);
}
