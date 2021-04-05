using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class BallConnected : Ball
    {
        private Vector2Int gridPos;
            
        private SpringJoint2D joint;

        private Animator _animator;

        [SerializeField] private bool hanging;
        public bool Hanging => hanging;

        public Vector2Int GridPos => gridPos;

        public void HangUp()
        {
            hanging = true;
            joint.enabled = true;
        }

        private void Awake()
        {
            joint = GetComponent<SpringJoint2D>();
            _animator = GetComponent<Animator>();
        }
        
        private void Start()
        {
            joint.connectedAnchor = transform.position;
        }

        public void SetOnGrid(Vector2Int gridPos)
        {
            this.gridPos = gridPos;
        }

        /// <summary>
        /// Лопнуть шар и добавить очки
        /// </summary>
        public void PopBall()
        {
            gameObject.layer = Constants.DROPPED_LAYER;
            _animator.enabled = true;
            FindObjectOfType<ScoreManager>().AddToScore(Type.Score);
        }

        /// <summary>
        /// Уронить шар
        /// </summary>
        public void FallOff()
        {
            gameObject.layer = Constants.DROPPED_LAYER;
            hanging = false;
            joint.enabled = false;
        }

        /// <summary>
        /// Используется в анимации
        /// </summary>
        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}