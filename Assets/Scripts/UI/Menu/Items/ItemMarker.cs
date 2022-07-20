using BattleCity.UI.Menu;
using UnityEngine;
using UnityEngine.UI;

namespace BattleCity.Menu.Items
{
    public class ItemMarker : MonoBehaviour
    {
        [SerializeField]
        private Image _marker;
        [SerializeField]
        private bool _isSelected;
        [SerializeField]
        private MenuActionTypes _actionType;
        [SerializeField]
        private int _selectionIndex;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                _marker.enabled = _isSelected;
            }
        }

        public MenuActionTypes ActionType => _actionType;

        public int SelectionIndex => _selectionIndex;

        private void Start()
        {
            if (_isSelected)
            {
                IsSelected = _isSelected;
            }
        }

        public void SetMarker(Sprite marker)
        {
            _marker.sprite = marker;
        }
    }
}