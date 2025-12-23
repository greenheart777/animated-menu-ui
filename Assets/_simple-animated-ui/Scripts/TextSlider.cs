using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextSlider : MonoBehaviour
{
    [SerializeField] GameObject[] sliderObjects;
    [Space]
    [SerializeField] private TMP_Text pageTexts;
    [SerializeField] private Button prevButtons;
    [SerializeField] private Button nextButtons;

    private int currentIndex = 0;
    [SerializeField] private bool loopEnabled = true; // Флаг для включения/выключения зацикливания

    void Start()
    {
        if (sliderObjects == null || sliderObjects.Length == 0)
        {
            Debug.LogError("SliderObjects array is empty or not assigned!");
            return;
        }

        prevButtons.onClick.AddListener(ShowPrevious);
        nextButtons.onClick.AddListener(ShowNext);

        UpdateSliderObjects();
        UpdatePageText();
        UpdateButtonsInteractable();
    }

    void UpdateSliderObjects()
    {
        for (int i = 0; i < sliderObjects.Length; i++)
        {
            sliderObjects[i].SetActive(i == currentIndex);
        }
    }

    void UpdatePageText()
    {
        if (pageTexts != null)
        {
            pageTexts.text = $"{currentIndex + 1} из {sliderObjects.Length}";
        }
    }

    void UpdateButtonsInteractable()
    {
        if (!loopEnabled) // Если зацикливание выключено, управляем активностью кнопок
        {
            if (prevButtons != null)
            {
                prevButtons.interactable = currentIndex > 0;
            }

            if (nextButtons != null)
            {
                nextButtons.interactable = currentIndex < sliderObjects.Length - 1;
            }
        }
        else // Если зацикливание включено, кнопки всегда активны
        {
            if (prevButtons != null)
            {
                prevButtons.interactable = true;
            }

            if (nextButtons != null)
            {
                nextButtons.interactable = true;
            }
        }
    }

    public void ShowNext()
    {
        if (sliderObjects.Length == 0) return;

        if (loopEnabled)
        {
            // Зацикленный переход
            currentIndex = (currentIndex + 1) % sliderObjects.Length;
        }
        else
        {
            // Обычный переход с проверкой границ
            if (currentIndex < sliderObjects.Length - 1)
            {
                currentIndex++;
            }
            else
            {
                return; // Не делаем ничего, если достигли конца
            }
        }

        UpdateSliderObjects();
        UpdatePageText();
        UpdateButtonsInteractable();
    }

    public void ShowPrevious()
    {
        if (sliderObjects.Length == 0) return;

        if (loopEnabled)
        {
            currentIndex = (currentIndex - 1 + sliderObjects.Length) % sliderObjects.Length;
        }
        else
        {
            if (currentIndex > 0)
            {
                currentIndex--;
            }
            else
            {
                return; 
            }
        }

        UpdateSliderObjects();
        UpdatePageText();
        UpdateButtonsInteractable();
    }

    public void GoToPage(int index)
    {
        if (index >= 0 && index < sliderObjects.Length)
        {
            currentIndex = index;
            UpdateSliderObjects();
            UpdatePageText();
            UpdateButtonsInteractable();
        }
    }

    public void ResetToFirstPage()
    {
        currentIndex = 0;
        UpdateSliderObjects();
        UpdatePageText();
        UpdateButtonsInteractable();
    }

    public int GetCurrentPage()
    {
        return currentIndex;
    }

    public int GetTotalPages()
    {
        return sliderObjects.Length;
    }

    // Метод для переключения режима зацикливания
    public void SetLoopEnabled(bool enabled)
    {
        loopEnabled = enabled;
        UpdateButtonsInteractable();
    }

    // Метод для проверки режима зацикливания
    public bool IsLoopEnabled()
    {
        return loopEnabled;
    }
}