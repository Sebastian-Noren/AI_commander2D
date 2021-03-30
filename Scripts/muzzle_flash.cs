using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class muzzle_flash : MonoBehaviour
{
    // Start is called before the first frame update
    void FixedUpdate()
    {
        Destroy(gameObject, 2 * Time.deltaTime);
    }
}
