using UnityEngine;
using System.Collections;

public class AutoDestruct : MonoBehaviour
{
    void Start()
    {
        Destroy(this.gameObject, 0.5f);
    }
}
