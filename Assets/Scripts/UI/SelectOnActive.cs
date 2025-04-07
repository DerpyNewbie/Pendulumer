using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class SelectOnActive : MonoBehaviour
    {
        private Selectable _selectable;
        private GameObject _prevObject;

        private void Awake()
        {
            _selectable = GetComponent<Selectable>();
        }

        private void OnEnable()
        {
            _prevObject = EventSystem.current.currentSelectedGameObject;
            _selectable.Select();
        }

        private void OnDisable()
        {
            if (_prevObject == null || !_prevObject.activeInHierarchy) return;
            EventSystem.current.SetSelectedGameObject(_prevObject);
        }
    }
}