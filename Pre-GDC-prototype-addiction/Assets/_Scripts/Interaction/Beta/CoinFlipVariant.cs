using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Manager;
using UnityEngine;

public class CoinFlipVariant : MonoBehaviour
{
    private bool isFace = true;
    private bool isFlipping = false;
    // public Transform coinMesh;
    public KeyCode keycode;
    public List<AudioClip> flipSounds;
    public List<AudioClip> dropSounds;
    public List<ParticleSystem> dropParticles;

    public Transform shadow;
    
    void Update()
    {
        if (Input.GetKeyDown(keycode) && !isFlipping)
        {
            isFlipping = true;
            
            GameManager.Instance.resourceManager.ChangeTime(1);
            
            int flipAngle = Random.Range(5, 7) * 180 * (Random.Range(0, 2) * 2 - 1);
            isFace = ((flipAngle / 180) % 2 == 0) == isFace ? isFace : !isFace;
            
            Sequence flipSequence = DOTween.Sequence();
            flipSequence
                .Append(transform.DOLocalRotate(new Vector3(flipAngle + 50, 0, 0), 1f, RotateMode.FastBeyond360).SetEase(Ease.InQuad))
                .Insert(0, transform.DOLocalMoveY(transform.localPosition.y+6, 0.5f).SetEase(Ease.OutQuad).SetLoops(2, LoopType.Yoyo))
                .OnComplete((() =>
                {
                    isFlipping = false;
                    if (dropSounds.Count > 0) GameManager.Instance.audioManager.PlaySound(dropSounds[Random.Range(0, dropSounds.Count)]);
                    //TODO: Drop Particles
                }));
            
            flipSequence.Play();
            if (flipSounds.Count > 0) GameManager.Instance.audioManager.PlaySound(flipSounds[Random.Range(0, flipSounds.Count)]);

            shadow.DOScaleY(0.1f, 1 / 6f).SetLoops(6, LoopType.Yoyo);
            shadow.DOScaleX(shadow.transform.localScale.x - 0.1f, 0.5f).SetLoops(2, LoopType.Yoyo);
        }
    }
}