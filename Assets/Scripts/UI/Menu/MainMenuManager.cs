using BattleCity.Assistance.Static;
using BattleCity.Menu.Animations;
using BattleCity.Menu.Items;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace BattleCity.UI.Menu
{
    public enum MenuActionTypes : byte
    {
        Start_1Player = 0,
        Start_2Player = 1,
        Start_Construction = 2,
    }

    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField]
        private MainMenuAppearingAnimation _apperaingAnimation;

        [SerializeField, Space(15)]
        private Sprite _markerSprite;
        
        private ItemMarker[] _menuItems;
        private int _selectionNumber = 0;
        private bool _recieveInputEnabled = false;

        private void Start()
        {
            if (_apperaingAnimation != null && _apperaingAnimation.AnimationEnabled)
            {
                _apperaingAnimation.AnimationEnded += OnAppearAnimationEnded;
            }
            else
            {
                _recieveInputEnabled = true;
            }
            Initialize();
        }

        private void OnDisable()
        {
            UnsubscribeFromAnimation();
        }

        private void UnsubscribeFromAnimation()
        {
            if (_apperaingAnimation != null)
            {
                _apperaingAnimation.AnimationEnded -= OnAppearAnimationEnded;
            }
        }

        private void OnAppearAnimationEnded()
        {
            _recieveInputEnabled = true;
            UnsubscribeFromAnimation();
        }

        private void Initialize()
        {
            _menuItems = FindObjectsOfType<ItemMarker>();
            Array.Sort(_menuItems, new Comparison<ItemMarker>((item1, item2) => item1.SelectionIndex > item2.SelectionIndex ? 1 : 0));

            for (int i = 0; i < _menuItems.Length; i++)
            {
                _menuItems[i].SetMarker(_markerSprite);
            }

            try
            {
                ItemMarker selectedMarker = _menuItems.First(i => i.IsSelected);
                _selectionNumber = Array.IndexOf(_menuItems, selectedMarker);
            }
            catch (InvalidOperationException)
            {
                Debug.LogError("No selected item found, or no menu items found!");
            }
        }

        private void OnSelectionChanging(InputValue value)
        {
            if (_recieveInputEnabled)
            {
                int indexChange = Convert.ToInt32(value.Get<float>());
                int newIndex = _selectionNumber + indexChange;

                if (newIndex >= 0 && newIndex < _menuItems.Length)
                {
                    _menuItems[_selectionNumber].IsSelected = false;
                    _selectionNumber = newIndex;
                    _menuItems[_selectionNumber].IsSelected = true;
                }
            }
        }

        private void OnSelect(InputValue value)
        {
            if (_recieveInputEnabled)
            {
                switch (_menuItems[_selectionNumber].ActionType)
                {
                    case MenuActionTypes.Start_1Player:
                        GameStaticVariables.GameMode = SelectedGameMode.Mode_1Player;
                        SceneHelper.SwitchScene("GamePlay");
                        break;
                    case MenuActionTypes.Start_2Player:
                        GameStaticVariables.GameMode = SelectedGameMode.Mode_2Player;
                        SceneHelper.SwitchScene("GamePlay");
                        break;
                    case MenuActionTypes.Start_Construction:
#if UNITY_EDITOR
                        Debug.Log($"TODO: level editor");
#endif
                        break;
                }
            }   
        }
    }
}

