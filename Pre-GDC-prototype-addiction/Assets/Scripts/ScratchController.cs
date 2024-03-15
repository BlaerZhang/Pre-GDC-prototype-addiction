using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using DG.Tweening;
using ScratchCardAsset;
using UnityEngine;

public class ScratchController : MonoBehaviour
{
    [Header("Basic")]
    public SpriteRenderer spriteRenderer;
    public CinemachineVirtualCamera vCam;
    
    [Header("Scratch")]
    public float speed;
    public float pressure;
    public float scratchOffset;
    public float progressSpeed = 0f;
    private float previousProgress = 0f;
    private float currentProgress = 0f;

    [Header("Feedback")] 
    public bool feedback;
    public bool screenShake = false;
    public List<ParticleSystem> particles;
    public AudioClip soundLayer1;

    private ScratchCard card;
    private SpriteRenderer cardSprite;
    private EraseProgress eraseProgress;
    // Start is called before the first frame update
    void Start()
    {
        card = FindObjectOfType<ScratchCard>();
        cardSprite = card.transform.parent.Find("Scratch Surface Sprite").GetComponent<SpriteRenderer>();
        eraseProgress = FindObjectOfType<EraseProgress>();
    }

    // Update is called once per frame
    void Update()
    {   
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePosDamped;
        Vector2 mouseToScratcher = (Vector2)transform.position - mousePos;
        
        //Look Rotation
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseToScratcher);
        
        //Change Scale
        if (mouseToScratcher.magnitude <= spriteRenderer.bounds.size.y)
        {
            transform.localScale = new Vector3(1, mouseToScratcher.magnitude / spriteRenderer.bounds.size.y, 1);
        }
        //Move Towards
        else
        {
            transform.position += -(Vector3)mouseToScratcher.normalized * MathF.Pow(mouseToScratcher.magnitude,1f) * speed * Time.deltaTime;
        }
        
        //scratch
        if (Input.GetMouseButton(0))
        {
            card.ScratchHole(ConvertToScratchCardTexturePosition(transform.localPosition + (Vector3)mouseToScratcher.normalized * scratchOffset), pressure);
        }
        
        //Calculate Progress speed
        currentProgress = eraseProgress.GetProgress();
        progressSpeed = 10000 * (currentProgress - previousProgress) / Time.deltaTime;
        previousProgress = currentProgress;
        
        //Feedback
        if (!feedback) return;
        if (progressSpeed > 0.1f) 
        {
            if (particles != null)
            {
                foreach (var particle in particles)
                {
                    particle.Play();
                    vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = screenShake ? 1 : 0;
                    print("Particles!");
                }
            }
            //sound
        }
        
        else
        {
            if (particles != null)
            {
                foreach (var particle in particles)
                {
                    particle.Stop();
                    vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                }
            }
            //sound stop
        }
    }
    
    public Vector2 ConvertToScratchCardTexturePosition(Vector2 scratchPos)
    {
        Vector3 scratchCardOrigin = new Vector2(cardSprite.transform.position.x - cardSprite.sprite.bounds.size.x / 2,
            cardSprite.transform.position.y - cardSprite.sprite.bounds.size.y / 2);
        Vector2 relativePos = scratchPos - (Vector2)scratchCardOrigin;
        Vector2 uvPosition = new Vector2(relativePos.x / cardSprite.sprite.bounds.size.x, relativePos.y / cardSprite.sprite.bounds.size.y);

        Vector2 convertedPosition = new Vector2(Mathf.FloorToInt(uvPosition.x * cardSprite.sprite.texture.width),
            Mathf.FloorToInt(uvPosition.y * cardSprite.sprite.texture.height));

        return convertedPosition;
    }
}
