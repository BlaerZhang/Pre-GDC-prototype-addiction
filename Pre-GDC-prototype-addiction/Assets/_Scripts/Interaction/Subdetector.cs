using _Scripts.Manager;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class Subdetector : MonoBehaviour
    {
        // private DetectScratchArea detectScratchArea;
    
        // void Start()
        // {
        //     detectScratchArea = GetComponentInParent<DetectScratchArea>();
        // }

        private void OnMouseOver()
        {
            DetectScratchArea.isOverScratchArea = true;
        }

        private void OnMouseEnter()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.ScratchFieldHover);
        }

        private void OnMouseExit()
        {
            DetectScratchArea.isOverScratchArea = false;
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Idle);
        }

        private void OnMouseDrag()
        {
            if (GameManager.Instance.cursorManager.currentCursorType.Equals(CursorManager.CursorType.ScratchFieldHover))
                GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Scratching);
        }

        private void OnMouseDown()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Scratching);
        }

        private void OnMouseUp()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.ScratchFieldHover);
        }
    }
}
