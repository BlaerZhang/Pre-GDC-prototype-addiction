using System;
using System.Collections;
using System.Collections.Generic;
using ScratchCardAsset;
using UnityEngine;

public class ScratchController : MonoBehaviour
{
    [Header("Basic")]
    public SpriteRenderer spriteRenderer;
    
    [Header("Scratch")]
    public float speed;
    public float pressure;
    public float scratchOffset;
    public float progressSpeed = 0f;
    private float previousProgress = 0f;
    private float currentProgress = 0f;

    [Header("Feedback")] 
    public bool feedback;
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
        
        //Look Rotation
        transform.rotation = Quaternion.LookRotation(Vector3.forward, (Vector2)transform.position - mousePos);
        
        //Change Scale
        if (((Vector2)transform.position - mousePos).magnitude <= spriteRenderer.bounds.size.y)
        {
            transform.localScale = new Vector3(1,
                ((Vector2)transform.position - mousePos).magnitude / spriteRenderer.bounds.size.y, 1);
        }
        //Move Towards
        else
        {
            transform.position += (Vector3)(mousePos - (Vector2)transform.position) * speed * Time.deltaTime;
        }
        
        //scratch
        if (Input.GetMouseButton(0))
        {
            card.ScratchHole(ConvertToScratchCardTexturePosition(transform.localPosition + (Vector3)((Vector2)transform.position - mousePos).normalized * scratchOffset), pressure);
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
