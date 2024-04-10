using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFlip : MonoBehaviour
{
    private Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            rigidbody.AddForceAtPosition(new Vector3(0,0,Random.Range(-5,-10)),new Vector2(Random.Range(-1,1),Random.Range(-1,1)),ForceMode.Impulse);
        }
    }
}
