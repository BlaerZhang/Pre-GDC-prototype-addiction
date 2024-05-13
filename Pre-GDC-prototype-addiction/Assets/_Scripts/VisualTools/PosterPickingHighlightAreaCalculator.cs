using System;
using System.Collections;
using System.Collections.Generic;
using Abu;
using UnityEngine;

public class PosterPickingHighlightAreaCalculator : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _scrollViewBackground;
    private TutorialHighlight _highlight;

    private void Start()
    {
        _highlight = GetComponent<TutorialHighlight>();
    }

    void Update()
    {
        float yScale = _scrollViewBackground.transform.position.y - transform.position.y - _scrollViewBackground.bounds.size.y / 2;
        yScale = Mathf.Clamp(yScale, 0, Single.PositiveInfinity);
        transform.localScale = new Vector3(transform.localScale.x, yScale, transform.localScale.z);
        
        //turn on/off highlight
        if (yScale > 10 || yScale == 0) _highlight.enabled = false;
        else _highlight.enabled = true;
    }
}
