using _Scripts.Manager;
using UnityEngine;

namespace _Scripts.Interaction
{
    public class DetectScratchArea : MonoBehaviour
    {
        public static bool isOverCard = false;
        public static bool isOverScratchArea = false;

        private Subdetector subdetector;

        void Start()
        {
            subdetector = GetComponentInChildren<Subdetector>();
        }

        private void OnMouseOver()
        {
            isOverCard = true;
        }

        private void OnMouseEnter()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.DragAreaHover);
        }

        private void OnMouseDown()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Dragging);
        }

        private void OnMouseDrag()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Dragging);
        }

        private void OnMouseUp()
        {
            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.DragAreaHover);
        }

        private void OnMouseExit()
        {
            isOverCard = false;

            GameManager.Instance.cursorManager.SetCursor(CursorManager.CursorType.Idle);
        }
    }
}
