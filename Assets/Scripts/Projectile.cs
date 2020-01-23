using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float deathTimer = 3.0f;

    void Start()
    {
        Destroy(gameObject, deathTimer);
    }
}
