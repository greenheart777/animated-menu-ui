using DG.Tweening;

namespace SimpleAnimatedUI
{
    public interface IAnimator 
    {
        public Tween GetInAnimation();
        public Tween GetOutAnimation();

    } 
}
