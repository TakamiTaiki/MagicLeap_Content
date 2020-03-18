using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookUser : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Transform ui;
    void Update()
    {
        Vector3 aim = target.transform.position - ui.position;
        var look = Quaternion.LookRotation(-aim);
        ui.localRotation = look;

        aim = target.transform.position - transform.position;
        aim.y *= 0;
        float d = aim.magnitude;
        d -= 1f;
        d = Mathf.Clamp(d, 0.75f, d);
        ui.transform.position = transform.position + aim.normalized * d;

    }
}
