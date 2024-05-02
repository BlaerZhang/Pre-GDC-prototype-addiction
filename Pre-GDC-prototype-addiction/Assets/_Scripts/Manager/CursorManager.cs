using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CursorManager : MonoBehaviour
{
    public enum CursorType
    {
        Idle,
        Click,
        Dragging,
        DragAreaHover,
        ScratchFieldHover,
        Scratching
    }

    [Serializable]
    public class CursorData
    {
        public CursorType type;
        public Texture2D texture;
        public Vector2 hotspot = Vector2.zero;

        public CursorData(CursorType type, Texture2D texture, Vector2 hotspot)
        {
            this.type = type;
            this.texture = texture;
            this.hotspot = hotspot;
        }
    }

    [SerializeField] private List<CursorData> cursorData;

    [HideInInspector] public CursorType currentCursorType = CursorType.Idle;

    private Dictionary<CursorType, CursorData> cursorDictionary;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Confined;
        BuildCursorDictionary();
        SetCursor(CursorType.Idle);
    }

    private void BuildCursorDictionary()
    {
        cursorDictionary = new Dictionary<CursorType, CursorData>();
        foreach (CursorData data in cursorData)
        {
            cursorDictionary[data.type] = data;
        }
    }

    public void SetCursor(CursorType type)
    {
        if (cursorDictionary.TryGetValue(type, out CursorData data))
        {
            currentCursorType = type;
            Cursor.SetCursor(data.texture, data.hotspot, CursorMode.Auto);
        }
    }
}
