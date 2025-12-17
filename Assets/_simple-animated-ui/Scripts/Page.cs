using System;
using UnityEngine;

namespace SimpleAnimatedUI
{
    [Serializable]
    public class Page
    {
        public PageEnum PageID;
        public CanvasGroup Header;
        public CanvasGroup Body;
    }
}
