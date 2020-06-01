using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookUser : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 aim = transform.position - Camera.main.transform.position;
        var look = Quaternion.LookRotation(aim);
        transform.localRotation = look;
    }
}
