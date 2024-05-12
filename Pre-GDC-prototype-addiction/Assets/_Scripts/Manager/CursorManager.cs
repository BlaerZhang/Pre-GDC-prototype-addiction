using System;
using System.Collections.Generic;
using _Scripts.PlayerTools.Payphone;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Scripts.Manager
{
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

        private bool payphoneState = false;

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

        private void OnEnable()
        {
            PayphoneManager.onPhoneStateChanged += isInMessage => { payphoneState = isInMessage; };
        }
        
        private void OnDisable()
        {
            PayphoneManager.onPhoneStateChanged -= isInMessage => { payphoneState = isInMessage; };
        }

        private void BuildCursorDictionary()
        {
            cursorDictionary = new Dictionary<CursorType, CursorData>();
            foreach (CursorData data in cursorData)
            {
                cursorDictionary[data.type] = data;
            }
        }

        private void RestCursorToIdle()
        {
            cursorDictionary.TryGetValue(CursorType.Idle, out CursorData data);
            Cursor.SetCursor(data.texture, data.hotspot, CursorMode.ForceSoftware);
        }

        public void SetCursor(CursorType type)
        {
            if (cursorDictionary.TryGetValue(type, out CursorData data))
            {
                if (payphoneState)
                {
                    RestCursorToIdle();
                    return;
                }
                
                currentCursorType = type;
                Cursor.SetCursor(data.texture, data.hotspot, CursorMode.ForceSoftware);
            }
        }
    }
}
