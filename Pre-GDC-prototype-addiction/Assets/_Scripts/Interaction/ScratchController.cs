using System;
using System.Collections.Generic;
using Cinemachine;
using ScratchCardAsset;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace Interaction
{
    public class ScratchController : MonoBehaviour
    {
        [Header("Basic")]
        public SpriteRenderer spriteRenderer;
        public CinemachineVirtualCamera vCam;

        [Header("Scratch")]
        public float speed;
        public float pressure;
        public float scratchOffset;
        public float progressSpeed = 0f;
        private float previousProgress = 0f;
        private float currentProgress = 0f;

        [Header("Feedback")]
        public bool feedback;
        [Range(0,10)]
        public float screenShake = 0;
        public List<ParticleSystem> particles;
        public AudioClip soundLayer1;

        private ScratchCardAsset.ScratchCard card;
        private SpriteRenderer cardSprite;
        private EraseProgress eraseProgress;
        private PostProcessVolume postProcessVolume;

        private ChromaticAberration chromaticAberration;

        private void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;

            postProcessVolume = FindObjectOfType<PostProcessVolume>();
            postProcessVolume.profile.TryGetSettings(out chromaticAberration);
        }

        private void OnEnable()
        {
            MouseEnterDetector.onMouseEnterEvent += InitializeCurrentCard;
            MouseEnterDetector.onMouseExitEvent += RemoveCurrentCard;
        }

        private void OnDisable()
        {
            MouseEnterDetector.onMouseEnterEvent -= InitializeCurrentCard;
            MouseEnterDetector.onMouseExitEvent -= RemoveCurrentCard;
        }

        private void InitializeCurrentCard(ScratchCardAsset.ScratchCard currentCard)
        {
            card = currentCard;
            print(currentCard.name);
            // card = FindObjectOfType<ScratchCard>();
            cardSprite = card.transform.parent.Find("Scratch Surface Sprite").GetComponent<SpriteRenderer>();
            eraseProgress = card.gameObject.GetComponent<EraseProgress>();
            // eraseProgress = FindObjectOfType<EraseProgress>();
        }

        private void RemoveCurrentCard()
        {
            card = null;
        }

        // Update is called once per frame
        void Update()
        {
            if (card is null) return;
            MoveScratchTool();
        }

        private void MoveScratchTool()
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Vector2 mousePosDamped;
            Vector2 mouseToScratcher = (Vector2)transform.position - mousePos;

            //Look Rotation
            transform.rotation = Quaternion.LookRotation(Vector3.forward, mouseToScratcher);

            //Change Scale
            if (mouseToScratcher.magnitude <= spriteRenderer.bounds.size.y)
            {
                transform.localScale = new Vector3(1, mouseToScratcher.magnitude / spriteRenderer.bounds.size.y, 1);
            }
            //Move Towards
            else
            {
                transform.position += -(Vector3)mouseToScratcher.normalized * MathF.Pow(mouseToScratcher.magnitude,1f) * speed * Time.deltaTime;
            }

            //scratch
            if (Input.GetMouseButton(0))
            {
                card.ScratchHole(ConvertToScratchCardTexturePosition(transform.localPosition + (Vector3)mouseToScratcher.normalized * scratchOffset), pressure);
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
                vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = screenShake;
                chromaticAberration.intensity.value += Time.deltaTime * 2;
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
                vCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_AmplitudeGain = 0;
                if (chromaticAberration.intensity.value > 0) chromaticAberration.intensity.value -= Time.deltaTime;
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
}
