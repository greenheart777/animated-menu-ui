using UnityEngine;

namespace SimpleAnimatedUI
{
    public class OpenPageButton : MonoBehaviour
    {
        [SerializeField] private PageEnum pageToOpen;

        public void Open()
        {
            if (PageManager.Instance == null)
            {
                Debug.LogError("PageManager instance not found");
                return;
            }

            PageManager.Instance.OpenPage(pageToOpen);
        }
    } 
}
