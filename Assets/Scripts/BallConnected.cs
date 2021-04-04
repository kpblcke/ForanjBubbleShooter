using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class BallConnected : Ball
    {
        private Vector2Int gridPos;
            
        private SpringJoint2D joint;

        private Rigidbody2D _rigidbody2D;
        
        [SerializeField] private bool hanging;
        [SerializeField] private float wiggleMultiply = 20f;
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
        }
        
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            joint.connectedAnchor = transform.position;
        }

        public void SetOnGrid(Vector2Int gridPos)
        {
            this.gridPos = gridPos;
        }

        public void Wiggle(Vector2 force)
        {
            _rigidbody2D.AddForce(force * wiggleMultiply);
        }

        public void PopBall()
        {
            gameObject.SetActive(false);
            FindObjectOfType<ScoreManager>().AddToScore(Type.Score);
            Destroy(gameObject);
        }
        
        public void FallOff()
        {
            gameObject.layer = Constants.DROPPED_LAYER;
            hanging = false;
            joint.enabled = false;
            Destroy(gameObject, 3f);
        }
    }
}