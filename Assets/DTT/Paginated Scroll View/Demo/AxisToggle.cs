using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DTT.PaginatedScrollView.Demo
{
    /// <summary>
    /// Can be added to a toggle game object to emit a end of scroll value when toggled.
    /// </summary>
    public class AxisToggle : MonoBehaviour
    {
        /// <summary>
        /// The end of scroll behaviour value to emit.
        /// </summary>
        [SerializeField]
        private ScrollAxis _axis;

        /// <summary>
        /// The event emitting the value.
        /// </summary>
        [SerializeField]
        private ScrollAxisEvent _event;

        /// <summary>
        /// The toggle which should be checked.
        /// </summary>
        private Toggle _toggle;

        /// <summary>
        /// Assigns the toggle value and starts listening for toggle changes.
        /// </summary>
        private void Awake()
        {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(OnToggled);
        }

        /// <summary>
        /// Called when the toggle value is changed to emit the event.
        /// </summary>
        /// <param name="newValue">The new toggle value.</param>
        private void OnToggled(bool newValue) => _event.Invoke(_axis);

        /// <summary>
        /// The serializable unity event used to emit a value.
        /// </summary>
        [Serializable]
        private class ScrollAxisEvent : UnityEvent<ScrollAxis> {}
    }
}
