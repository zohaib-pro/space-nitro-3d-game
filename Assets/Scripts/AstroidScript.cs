using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstroidScript : MonoBehaviour
{

    private void OnTriggerEnter(Collider collider)
    {
        PlayerHealthScript ph = collider.GetComponent<PlayerHealthScript>();
        if (ph == null)
            return;

        ph.crash();
    }
}
