using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthScript : MonoBehaviour
{
    public void crash()
    {
        gameObject.SetActive(false);
    }
}
