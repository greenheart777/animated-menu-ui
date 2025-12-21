using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class ImageSlider : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] GameObject windowed;
    [SerializeField] GameObject fullscreen;
    [Space]
    [SerializeField] private TMP_Text[] pageTexts;
    [SerializeField] private Button[] prevButtons;
    [SerializeField] private Button[] nextButtons;
    [SerializeField] private Image displayWindowedImage;
    [SerializeField] private Image displayFullscreenImage;
    [Space]
    [Header("Settings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private List<Sprite> imagesWindowed = new List<Sprite>();
    [SerializeField] private List<Sprite> imagesFullscreen = new List<Sprite>();
    [Space]
    [Header("Initialization Settings")]
    [SerializeField] private float initialFadeDuration = 0.5f;
    [SerializeField] private Ease initialFadeEase = Ease.OutSine;
    [SerializeField] private float initialDelay = 0f;
    [SerializeField] private bool fadeInOnStart = true;

    private int currentIndex = 0;
    private bool isAnimating = false;
    private bool isFullscreen = false;

    private void Start()
    {
        Initialize();

        EnableWindowed();
        UpdateDisplay();
    }

    private void Initialize()
    {
        if (prevButtons != null && prevButtons.Length > 0)
        {
            for (int i = 0; i < prevButtons.Length; i++)
            {
                if (prevButtons[i] != null)
                {
                    prevButtons[i].onClick.AddListener(() => ShowPreviousImage());
                }
            }
        }

        if (nextButtons != null && nextButtons.Length > 0)
        {
            for (int i = 0; i < nextButtons.Length; i++)
            {
                if (nextButtons[i] != null)
                {
                    nextButtons[i].onClick.AddListener(() => ShowNextImage());
                }
            }
        }

        if (displayWindowedImage != null && imagesWindowed.Count > 0)
        {
            displayWindowedImage.sprite = imagesWindowed[0];
            Color color = displayWindowedImage.color;
            color.a = 0f;
            displayWindowedImage.color = color;
        }

        if (displayFullscreenImage != null && imagesFullscreen.Count > 0)
        {
            displayFullscreenImage.sprite = imagesFullscreen[0];
            Color color = displayFullscreenImage.color;
            color.a = 0f;
            displayFullscreenImage.color = color;
        }

        UpdatePageText();

        if (fadeInOnStart)
        {
            StartCoroutine(InitializedImagesFadeCoroutine());
        }
    }

    private IEnumerator InitializedImagesFadeCoroutine()
    {
        isAnimating = true;

        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        Sequence sequence = DOTween.Sequence();

        if (isFullscreen && displayFullscreenImage != null)
        {
            sequence.Join(displayFullscreenImage.DOFade(1f, initialFadeDuration)
                .SetEase(initialFadeEase));
        }
        else if (!isFullscreen && displayWindowedImage != null)
        {
            sequence.Join(displayWindowedImage.DOFade(1f, initialFadeDuration)
                .SetEase(initialFadeEase));
        }

        yield return sequence.WaitForCompletion();

        isAnimating = false;
    }

    private void OnDestroy()
    {
        if (displayWindowedImage != null)
        {
            displayWindowedImage.DOKill();
        }
        if (displayFullscreenImage != null)
        {
            displayFullscreenImage.DOKill();
        }
    }

    private void UpdateDisplay()
    {
        if (isAnimating) return;

        if (isFullscreen)
        {
            if (imagesFullscreen.Count > 0 && currentIndex < imagesFullscreen.Count)
            {
                UpdatePageText();
                StartCoroutine(ChangeImageCoroutine(imagesFullscreen[currentIndex], true));
            }
        }
        else
        {
            if (imagesWindowed.Count > 0 && currentIndex < imagesWindowed.Count)
            {
                UpdatePageText();
                StartCoroutine(ChangeImageCoroutine(imagesWindowed[currentIndex], false));
            }
        }
    }

    private void UpdatePageText()
    {
        if (pageTexts != null && pageTexts.Length > 0)
        {
            int totalImages = isFullscreen ? imagesFullscreen.Count : imagesWindowed.Count;
            string text = $"{currentIndex + 1} из {totalImages}";
            for (int i = 0; i < pageTexts.Length; i++)
            {
                if (pageTexts[i] != null)
                {
                    pageTexts[i].text = text;
                }
            }
        }
    }

    private IEnumerator ChangeImageCoroutine(Sprite newSprite, bool forFullscreen)
    {
        isAnimating = true;

        Image targetImage = forFullscreen ? displayFullscreenImage : displayWindowedImage;

        if (targetImage == null)
        {
            isAnimating = false;
            yield break;
        }

        Sequence sequence = DOTween.Sequence();

        sequence.Join(targetImage.DOFade(0f, fadeDuration / 2f));

        sequence.AppendCallback(() =>
        {
            targetImage.sprite = newSprite;
        });

        sequence.Join(targetImage.DOFade(1f, fadeDuration / 2f));

        yield return sequence.WaitForCompletion();

        isAnimating = false;
    }

    public void ShowNextImage()
    {
        if (isAnimating) return;

        int totalImages = isFullscreen ? imagesFullscreen.Count : imagesWindowed.Count;
        if (totalImages == 0) return;

        currentIndex++;
        if (currentIndex >= totalImages)
        {
            currentIndex = 0;
        }

        UpdateDisplay();
    }

    public void ShowPreviousImage()
    {
        if (isAnimating) return;

        int totalImages = isFullscreen ? imagesFullscreen.Count : imagesWindowed.Count;
        if (totalImages == 0) return;

        currentIndex--;
        if (currentIndex < 0)
        {
            currentIndex = totalImages - 1;
        }

        UpdateDisplay();
    }

    public void GoToImage(int index)
    {
        if (isAnimating) return;

        int totalImages = isFullscreen ? imagesFullscreen.Count : imagesWindowed.Count;
        if (totalImages > 0 && index >= 0 && index < totalImages)
        {
            currentIndex = index;
            UpdateDisplay();
        }
    }

    public void EnableWindowed()
    {
        if (windowed != null) windowed.SetActive(true);
        if (fullscreen != null) fullscreen.SetActive(false);
        isFullscreen = false;

        if (imagesWindowed.Count > 0 && currentIndex < imagesWindowed.Count)
        {
            displayWindowedImage.sprite = imagesWindowed[currentIndex];
            displayWindowedImage.color = new Color(displayWindowedImage.color.r,
                displayWindowedImage.color.g,
                displayWindowedImage.color.b,
                1f);
        }

        UpdatePageText();
    }

    public void EnableFullscreen()
    {
        if (windowed != null) windowed.SetActive(false);
        if (fullscreen != null) fullscreen.SetActive(true);
        isFullscreen = true;

        if (imagesFullscreen.Count > 0 && currentIndex < imagesFullscreen.Count)
        {
            displayFullscreenImage.sprite = imagesFullscreen[currentIndex];
            displayFullscreenImage.color = new Color(displayFullscreenImage.color.r,
                displayFullscreenImage.color.g,
                displayFullscreenImage.color.b,
                1f);
        }

        UpdatePageText();
    }
}