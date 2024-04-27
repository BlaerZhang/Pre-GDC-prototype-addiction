using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SpriteShadow : MonoBehaviour
{
    public Vector2 shadowOffset = new Vector2(0.2f, -0.2f);
    public Color shadowColor = new Color(0.1015625f, 0.1015625f, 0.1015625f, 1);
    public Color hoverShadowColor = Color.magenta;

    private SpriteRenderer _shadowParentSpriteRenderer;
    private SpriteRenderer _shadow;

    [Button("Initialize")]
    void Init()
    {
        _shadowParentSpriteRenderer = GetComponent<SpriteRenderer>();
        if (GetComponent<SortingGroup>() == null) this.AddComponent<SortingGroup>().sortingOrder = _shadowParentSpriteRenderer.sortingOrder;
        if (transform.Find("Shadow") == null)
        {
            _shadow = new GameObject("Shadow").AddComponent<SpriteRenderer>();
            _shadow.transform.parent = this.transform;
        }
        else
        {
            _shadow = transform.Find("Shadow").GetComponent<SpriteRenderer>();
        }
        _shadow.sprite = _shadowParentSpriteRenderer.sprite;
        _shadow.color = shadowColor; //reference temp look in editor
        _shadow.sortingOrder = -100;
        _shadow.transform.position = this.transform.position + (Vector3)shadowOffset;
        _shadow.transform.localScale = Vector3.one;
        _shadow.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    
    void Start()
    {
        Init();
        _shadow.material = new Material(Shader.Find("Custom/ColorFillShader"));
        _shadow.material.color = shadowColor;
    }

    void Update()
    {
        _shadow.transform.position = this.transform.position + (Vector3)shadowOffset;
    }
}
