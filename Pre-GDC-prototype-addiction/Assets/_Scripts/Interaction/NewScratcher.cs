using System;
using System.Collections.Generic;
using Cinemachine;
using ScratchCardAsset;
using ScratchCardAsset.Core;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Interaction
{
    public class NewScratcher : MonoBehaviour
    {
        [Header("Basic")]
        [HideInInspector] public CinemachineVirtualCamera vCam;

        // [Header("Scratch")]
        // public float speed;
        // public float pressure;
        // public float scratchOffset;
        public float progressSpeed = 0f;
        private float previousProgress = 0f;
        private float currentProgress = 0f;
        private float mouseDir = 0f;

        [Header("Feedback")]
        public bool feedback;
        [Range(0,10)]
        public float screenShake = 0;
        [Range(0,1)]
        public float chromaticAberrationAmount = 0;
        public float mouseMovementDeadZone = 0.01f;
        public List<ParticleSystem> particles;
        // public AudioClip soundLayer1;

        [Header("Card")]
        public ScratchCard card;
        public SpriteRenderer cardSprite;
        public EraseProgress eraseProgress;
        private PostProcessVolume postProcessVolume;
        private ChromaticAberration chromaticAberration;

        private void Start()
        {
            postProcessVolume = FindObjectOfType<PostProcessVolume>();
            postProcessVolume.profile.TryGetSettings(out chromaticAberration);
            vCam = GameObject.Find("Buy Card vCam").GetComponent<CinemachineVirtualCamera>();

            foreach (var particle in particles)
            {
                particle.Stop();
            }
        }

        // private void OnEnable()
        // {
        //     eraseProgress.OnProgress += OnProgress;
        // }
        //
        // private void OnDisable()
        // {
        //     eraseProgress.OnProgress -= OnProgress;
        // }
        
        void Update()
        {
            if (card is null) return;
            OnProgress(0);
        }

        private void OnProgress(float progress)
        {
            //Calculate Progress speed
            currentProgress = eraseProgress.GetProgress();
            progressSpeed = 1000000 * (currentProgress - previousProgress) * Time.deltaTime;
            previousProgress = currentProgress;
            
            // //scratch
            // if (Input.GetMouseButton(0))
            // {
            //     card.ScratchHole(ConvertToScratchCardTexturePosition(Camera.main.ScreenToWorldPoint(Input.mousePosition)), pressure);
            // }
            
            //Feedback
            if (!feedback) return;
            
            float mouseX = Input.GetAxis ("Mouse X") * 1.5f;
            float mouseY = Input.GetAxis ("Mouse Y") * 1.5f;
            mouseDir = (mouseX <= mouseMovementDeadZone && mouseY <= mouseMovementDeadZone) ? mouseDir : Mathf.Atan2(mouseY, mouseX) * Mathf.Rad2Deg;

            if (progressSpeed > 0)  
            { 
                if (particles != null)
                {
                    foreach (var particle in particles)
                    {
                        if (!particle.isPlaying) particle.Play();
                        particle.transform.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        particle.transform.rotation = Quaternion.Euler(180 - mouseDir, 90, -90);
                        // print("Particles!");
                    }
                }
                
                vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = screenShake; 
                chromaticAberration.intensity.value += Time.deltaTime * chromaticAberrationAmount; 
                
                //TODO: sound
            }
            else
            {
                foreach (var particle in particles)
                {
                    if (particle.isPlaying) particle.Stop();
                }
                
                vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                if (chromaticAberration.intensity.value > 0) chromaticAberration.intensity.value -= Time.deltaTime * 1f;
                
                //TODO:sound stop  
            }
        }
        
        // public Vector2 ConvertToScratchCardTexturePosition(Vector2 scratchPos)
        // {
        //     Vector3 scratchCardOrigin = new Vector2(cardSprite.transform.position.x - cardSprite.sprite.bounds.size.x / 2,
        //         cardSprite.transform.position.y - cardSprite.sprite.bounds.size.y / 2);
        //     Vector2 relativePos = scratchPos - (Vector2)scratchCardOrigin;
        //     Vector2 uvPosition = new Vector2(relativePos.x / cardSprite.sprite.bounds.size.x, relativePos.y / cardSprite.sprite.bounds.size.y);
        //
        //     Vector2 convertedPosition = new Vector2(Mathf.FloorToInt(uvPosition.x * cardSprite.sprite.texture.width),
        //         Mathf.FloorToInt(uvPosition.y * cardSprite.sprite.texture.height));
        //
        //     return convertedPosition;
        // }
    }
}
