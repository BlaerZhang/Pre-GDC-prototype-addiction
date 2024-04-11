using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CoinFlip : MonoBehaviour
{
    private Rigidbody rigidbody;
    private bool isFace = true;
    private bool isFlipping = false;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isFlipping)
        {
            isFlipping = true;
            
            int flipAngle = Random.Range(5, 7) * 180;
            isFace = ((flipAngle / 180) % 2 == 0) == isFace ? isFace : !isFace;
            
            Sequence flipSequence = DOTween.Sequence();
            flipSequence
                .Append(transform.DOLocalRotate(new Vector3(0, flipAngle, 0), 1f, RotateMode.FastBeyond360).SetEase(Ease.InQuad))
                .Insert(0, transform.DOScaleX(1.5f, 0.5f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo))
                .Insert(0, transform.DOScaleY(1.5f, 0.5f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo))
                .OnComplete((() => { isFlipping = false; }));

            flipSequence.Play();
        }
    }
}
