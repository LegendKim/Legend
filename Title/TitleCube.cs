using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleCube : MonoBehaviour
{
    private void FixedUpdate()
    {
        this.transform.Rotate(1.0f, 1.0f, 1.0f);
    }
}
