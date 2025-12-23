using System;
using UnityEngine;

namespace SimpleAnimatedUI
{
    [Serializable]
    public class PageConfigData
    {
        public float pageFadeInDuration = 0.4f;
        public string fadeInEase = "InSine";
        public float pageFadeOutDuration = 0.7f;
        public string fadeOutEase = "OutSine";

        public DG.Tweening.Ease GetFadeInEase()
        {
            return ParseEase(fadeInEase, DG.Tweening.Ease.InSine);
        }

        public DG.Tweening.Ease GetFadeOutEase()
        {
            return ParseEase(fadeOutEase, DG.Tweening.Ease.InSine);
        }

        private DG.Tweening.Ease ParseEase(string easeName, DG.Tweening.Ease defaultEase)
        {
            try
            {
                return (DG.Tweening.Ease)Enum.Parse(typeof(DG.Tweening.Ease), easeName);
            }
            catch
            {
                Debug.LogWarning($"Unable to recognize Ease: {easeName}. Using default value.");
                return defaultEase;
            }
        }
    }
}