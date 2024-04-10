using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowToy : MonoBehaviour
{
    public Transform targetToFollow;
    
    void Start()
    {
        
    }
    
    void Update()
    {
        transform.position = new Vector3(targetToFollow.position.x, targetToFollow.position.y, transform.position.z);
    }
}
