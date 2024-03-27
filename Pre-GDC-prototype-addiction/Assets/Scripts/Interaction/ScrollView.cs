using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class ScrollView : MonoBehaviour
{
    public Vector2 scrollLimits;
    public float currentScrollSpeed = 0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentScrollSpeed = Mathf.Abs(currentScrollSpeed) > Mathf.Abs(Input.GetAxis("Mouse ScrollWheel"))
            ? currentScrollSpeed * 0.99f
            : Input.GetAxis("Mouse ScrollWheel");
        
        transform.position += Vector3.up * currentScrollSpeed * Time.deltaTime * 300f;

        if (Mathf.Abs(currentScrollSpeed) < 0.0001f) currentScrollSpeed = 0;

        transform.position = new Vector3(transform.position.x,
            Mathf.Clamp(transform.position.y, scrollLimits.x, scrollLimits.y),
            transform.position.z);
    }
    
}