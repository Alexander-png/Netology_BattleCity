using BattleCity.Managers;
using BattleCity.Managers.Game;
using System.Collections;
using UnityEngine;

namespace BattleCity.Animations
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class LiteAnimation : MonoBehaviour
    {
        private Coroutine _animationCoroutine;

        [SerializeField]
        private SpriteRenderer _sprite;
        [SerializeField]
        private Sprite[] _frames;
        [SerializeField]
        private bool _loop;
        [SerializeField, Min(0f)]
        public float _delay;
        [SerializeField]
        public LiteAnimationTypes _type;

        public LiteAnimationTypes Type => _type;

        public event LiteAnimationEvents AnimationEnded;

        private void Start()
        {
            StartAnimation();
        }

        public void StartAnimation()
        {
            if (_animationCoroutine == null)
            {
                _animationCoroutine = StartCoroutine(PlayAnimation());
            }
        }

        private IEnumerator PlayAnimation()
        {
            int counter = 0;
            _sprite.sprite = _frames[counter];

            while (counter < _frames.Length)
            {
                if (enabled)
                {
                    yield return new WaitForSeconds(_delay);
                    counter++;
                    if (counter < _frames.Length)
                    {
                        _sprite.sprite = _frames[counter];
                    }
                    if (_loop && counter == _frames.Length)
                    {
                        counter = -1;
                    }
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
            AnimationEnded?.Invoke();
            Destroy(gameObject);
        }

        public void SetVisible(bool value)
        {
            _sprite.enabled = value;
            enabled = value;
        }

        public delegate void LiteAnimationEvents();
    }
}
