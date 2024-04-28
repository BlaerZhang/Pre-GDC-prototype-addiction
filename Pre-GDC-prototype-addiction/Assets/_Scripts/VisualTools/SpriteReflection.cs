using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class SpriteReflection : MonoBehaviour
{
    public float reflectionTimeInterval = 5f;
    public float reflectionDuration = 1f;
    public float reflectionAngle = -15f;
    public Sprite reflectionSprite;
    [Range(0, 1)] public float reflectionTransparency = 0.5f;

    private SpriteRenderer spriteRenderer;
    private GameObject reflectionParent;
    
    void Start()
    {
        reflectionParent = transform.Find("Reflection Parent").gameObject;
        spriteRenderer = GetComponent<SpriteRenderer>();
        reflectionParent.transform.localPosition = new Vector3(spriteRenderer.sprite.texture.width, 0, 0);
        InvokeRepeating("Reflect", Random.Range(0f, 5f), reflectionTimeInterval);
    }

    [Button("Initialize")]
    void Init()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (GetComponent<SpriteMask>() == null) this.AddComponent<SpriteMask>().sprite = spriteRenderer.sprite;
        if(GetComponent<SortingGroup>() == null) this.AddComponent<SortingGroup>().sortingOrder = spriteRenderer.sortingOrder;

        if (transform.Find("Reflection Parent") != null) return;
        
        reflectionParent = new GameObject("Reflection Parent");
        reflectionParent.transform.parent = this.transform;
        reflectionParent.transform.localPosition = Vector3.zero;
            
        CreateReflection(-0.5f);
        CreateReflection(1);
        CreateReflection(0.5f);
        
    }

    void CreateReflection(float XPos)
    {
        GameObject square = new GameObject("Square");
        square.transform.parent = reflectionParent.transform;

        SpriteRenderer reflectionRenderer = square.AddComponent<SpriteRenderer>();
        reflectionRenderer.sprite = reflectionSprite;
        reflectionRenderer.color = new Color(1, 1, 1, reflectionTransparency);
        reflectionRenderer.drawMode = SpriteDrawMode.Sliced;
        reflectionRenderer.size = new Vector2(Random.Range(0.05f, 0.4f), 20);
        reflectionRenderer.sortingOrder = 100;
        reflectionRenderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        reflectionRenderer.transform.rotation = Quaternion.Euler(new Vector3(0, 0, reflectionAngle));
        reflectionRenderer.transform.localPosition = new Vector3(XPos, 0, 0);
    }

    void Reflect()
    {
        reflectionParent.transform.localPosition = new Vector3(spriteRenderer.bounds.size.x * 2.5f, 0, 0);
        reflectionParent.transform.DOLocalMoveX(-spriteRenderer.bounds.size.x * 2.5f, reflectionDuration).SetEase(Ease.Linear);
    }
}
