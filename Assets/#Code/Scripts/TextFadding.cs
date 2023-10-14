using System.Collections;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]

public class TextFading : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 2.0f;
    [SerializeField] private float delayBetweenCycles = 1.0f;

    private TextMeshProUGUI _textMesh;
    private Color _initialColor;
    private Material _textMaterial;

    private void Start()
    {
        _textMesh = GetComponent<TextMeshProUGUI>();
        _textMaterial = _textMesh.fontMaterial;
        _initialColor = _textMesh.color;

        StartCoroutine(FadeInOutCycle());
    }

    private IEnumerator FadeInOutCycle()
    {
        while (true)
        {
            // Затухание
            yield return StartCoroutine(FadeText(_initialColor, new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0f), fadeDuration));

            yield return new WaitForSeconds(delayBetweenCycles);

            // Возврат к исходной прозрачности
            yield return StartCoroutine(FadeText(new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0f), _initialColor, fadeDuration));
        }
    }

    private IEnumerator FadeText(Color startColor, Color targetColor, float duration)
    {
        float startTime = Time.time;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            _textMesh.color = Color.Lerp(startColor, targetColor, t);
            _textMaterial.SetColor(ShaderUtilities.ID_FaceColor, _textMesh.color);

            yield return null;
        }

        // Убедимся, что текст заканчивает на нужном цвете
        _textMesh.color = targetColor;
        _textMaterial.SetColor(ShaderUtilities.ID_FaceColor, _textMesh.color);
    }
}
