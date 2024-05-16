using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class InfiniteRotation : MonoBehaviour
{
    public float rotateDegreeLimitation;
    public float rotateSpeed;

    void Start()
    {
        float rotateDuration = rotateDegreeLimitation * 2 / rotateSpeed;

        int direction = Random.value > 0.5f ? 1 : -1;

        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - direction * rotateDegreeLimitation);
        transform.DORotate(new Vector3(0, 0, transform.eulerAngles.z + direction * rotateDegreeLimitation), rotateDuration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }
}
