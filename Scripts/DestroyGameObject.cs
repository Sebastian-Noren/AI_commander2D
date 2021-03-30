using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    public float lifeTime; // how long the object are allowed to live

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
