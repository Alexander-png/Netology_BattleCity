using System.Collections;
using UnityEngine;

namespace BattleCity.Menu.Animations
{
    public class MainMenuAppearingAnimation : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _transform;
        [SerializeField]
        private bool _animationEnabled = true;
        [SerializeField]
        private Vector3 _startPosition;
        [SerializeField]
        private Vector3 _endPosition;

        public bool AnimationEnabled => _animationEnabled;

        public event AnimationEvents AnimationEnded;

        private void Start()
        {
            if (_animationEnabled)
            {
                StartCoroutine(AppearingAnimation());
            }
        }

        private void OnDisable()
        {
            AnimationEnded = null;
        }

        private IEnumerator AppearingAnimation()
        {
            _transform.localPosition = _startPosition;

            while (true)
            {
                Vector3 nextPosition = Vector3.MoveTowards(_transform.localPosition, _endPosition, 450f * Time.deltaTime);
                _transform.localPosition = nextPosition;
                yield return new WaitForEndOfFrame();
                if (Vector3.Distance(_transform.localPosition, _endPosition) <= 1f)
                {
                    break;
                }    
            }
            _transform.localPosition = _endPosition;
            AnimationEnded?.Invoke();
        }

        public delegate void AnimationEvents();
    }
}


