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
        
        private List<BallConnected> parents = new List<BallConnected>();

        [SerializeField]
        private List<BallConnected> neighboor = new List<BallConnected>();

        [SerializeField] private bool hanging;
        [SerializeField] private float wiggleMultiply = 20f;

        public List<BallConnected> Parents => parents;

        public List<BallConnected> Neighboor => neighboor;

        public bool Hanging => hanging;

        public Vector2Int GridPos => gridPos;

        public void SetParents(List<BallConnected> newParents)
        {
            parents = newParents;
        }

        public void SetNeighboors(List<BallConnected> newNeighboors)
        {
            neighboor = newNeighboors;
        }

        public void AddNeighboor(BallConnected newNeighboor)
        {
            neighboor.Add(newNeighboor);
        }
        private void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            joint = GetComponent<SpringJoint2D>();
            joint.connectedAnchor = transform.position;
        }

        public void SetOnGrid(int x, int y)
        {
            gridPos = new Vector2Int(x, y);
        }

        public void Wiggle(Vector2 force)
        {
            _rigidbody2D.AddForce(force * wiggleMultiply);
        }

        public void PopBall()
        {
            FindObjectOfType<BallGrid>().RemoveBall(this);
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        public void ConnectBall(Vector2 atPosition, BallType type)
        {
            FindObjectOfType<BallGrid>().ConnectBall(this, atPosition, type);
        }
    }
}