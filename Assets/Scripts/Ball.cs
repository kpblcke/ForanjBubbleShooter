using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class Ball : MonoBehaviour
    {
        private BallType _type;
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        public BallType Type => _type;

        private void Awake()
        {
            if (!_spriteRenderer)
            {
                _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }

        protected void InitColor()
        {
            _spriteRenderer.sprite = _type.BallSprite;
        }

        public void ChangeType(BallType newType)
        {
            _type = newType;
            _spriteRenderer.sprite = _type.BallSprite;
        }
    }
}