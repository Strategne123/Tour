using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Vrs.Internal
{
    public class SpriteButtonGaze : MonoBehaviour, IVrsGazeResponder
    {
        [SerializeField] private Image answerBackground;
        private TextMeshProUGUI textMesh;

        private bool mGazeAt = false;
        private Coroutine changeColorCoroutine;
        private Coroutine changeTextVisibilityCoroutine;
        private Coroutine changeBackgroundVisibilityCoroutine;

        void Start()
        {
            textMesh = answerBackground.GetComponentInChildren<TextMeshProUGUI>();
            SetInitialTransparency();
            SetGazedAt(true); // временно
        }

        private void SetInitialTransparency()
        {
            Color textColor = textMesh.color;
            textColor.a = 1f;
            textMesh.color = textColor;

            Color backgroundColor = answerBackground.color;
            backgroundColor.a = 1f;
            answerBackground.color = backgroundColor;
        }

        public void SetGazedAt(bool gazedAt)
        {
            if (mGazeAt != gazedAt)
            {
                mGazeAt = gazedAt;
                if (changeColorCoroutine != null)
                {
                    StopCoroutine(changeColorCoroutine);
                }
                if (changeTextVisibilityCoroutine != null)
                {
                    StopCoroutine(changeTextVisibilityCoroutine);
                }
                if (changeBackgroundVisibilityCoroutine != null)
                {
                    StopCoroutine(changeBackgroundVisibilityCoroutine);
                }

                Color targetColor = gazedAt ? new Color(0, 1, 0, 0.33f) : new Color(0, 0, 0, 0);
                changeColorCoroutine = StartCoroutine(ChangeColorGradually(targetColor));
                float targetAlphaText = 1.0f;
                changeTextVisibilityCoroutine = StartCoroutine(ChangeTextVisibilityGradually(targetAlphaText));
                float targetAlphaBackground = gazedAt ? answerBackground.color.a : 0f;
                changeBackgroundVisibilityCoroutine = StartCoroutine(ChangeBackgroundVisibilityGradually(targetAlphaBackground, gazedAt));
            }
        }

        private IEnumerator ChangeColorGradually(Color targetColor)
        {
            float duration = 0.33f;
            float elapsedTime = 0;
            SpriteShapeRenderer renderer = GetComponent<UnityEngine.U2D.SpriteShapeRenderer>();
            Color startColor = renderer.color;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                renderer.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                yield return null;
            }

            renderer.color = targetColor;
        }

        private IEnumerator ChangeTextVisibilityGradually(float targetAlpha)
        {
            float duration = 0.33f;
            float elapsedTime = 0;
            Color startColor = textMesh.color;
            Color targetColor = new Color(startColor.r, startColor.g, startColor.b, targetAlpha);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                textMesh.color = Color.Lerp(startColor, targetColor, elapsedTime / duration);
                yield return null;
            }

            textMesh.color = targetColor;
        }

        private IEnumerator ChangeBackgroundVisibilityGradually(float targetAlpha, bool gazedAt)
        {
            float duration = 0.33f;
            float elapsedTime = 0;
            Color originalColor = answerBackground.color;
            Color targetColor = new Color(originalColor.r, originalColor.g, originalColor.b, gazedAt ? 0.78f : 0f);

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                answerBackground.color = Color.Lerp(originalColor, targetColor, elapsedTime / duration);
                yield return null;
            }

            answerBackground.color = targetColor;
        }

        public bool isGazedAt()
        {
            return mGazeAt;
        }

        public void OnGazeEnter()
        {
            // SetGazedAt(true); // временно
        }

        public void OnGazeExit()
        {
            // SetGazedAt(false); // временно
        }

        public void OnGazeTrigger() { }

        public void OnUpdateIntersectionPosition(Vector3 position) { }

    }
}
