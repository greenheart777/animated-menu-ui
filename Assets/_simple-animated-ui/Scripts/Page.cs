using System;
using UnityEngine;

namespace SimpleAnimatedUI
{
    [Serializable]
    public class Page
    {
        public PageEnum Id;
        public CanvasGroup Header;
        public CanvasGroup Body;
    }
}
