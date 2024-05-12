using System;
using _Scripts.Manager;
using UnityEngine;

namespace _Scripts.Interaction.Beta
{
    public class Window : MonoBehaviour
    {
        private Animator windowAnimator;
        private int windowIndex = 0; //Window Index: 0=morning; 1=noon; 2=night

        private void OnEnable()
        {
            windowAnimator = GetComponent<Animator>();
            ResourceManager.OnTimeChanged += UpdateWindowState;
        }

        private void OnDisable()
        {
            ResourceManager.OnTimeChanged -= UpdateWindowState;
        }
    
        void UpdateWindowState(DateTime currentDateTime)
        {
            switch (currentDateTime.Hour)
            {
                case <4:
                    windowIndex = 2;
                    break;
                case <14:
                    windowIndex = 0;
                    break;
                case <20:
                    windowIndex = 1;
                    break;
                case >=20:
                    windowIndex = 2;
                    break;
            }

            windowAnimator.SetInteger("Window Index", windowIndex);
        }
    }
}
