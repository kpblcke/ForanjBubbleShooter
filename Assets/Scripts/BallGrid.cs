using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class BallGrid : MonoBehaviour
    {
        [SerializeField] 
        private BallConnected ballPref;
        [SerializeField] 
        private List<BallType> loadBallTypes;

        [SerializeField] private Transform startSpawn;

        [SerializeField] private float xOffset = 0.5f;
        [SerializeField] private float yOffset = 0.42f;
        [SerializeField] private float divOffset = 0.25f;
        [SerializeField] private string levelName = "Level2";
        private List<BallConnected> allBalls;
        
        private void Start()
        { 
            GenerateBalls();
        }

        public List<BallConnected> GenerateBalls()
        {
            allBalls = new List<BallConnected>();
            var textFile = Resources.Load<TextAsset>(levelName);
            List<string> lines = new List<string>(textFile.text.Split('\n'));
            for(int l = 0; l < lines.Count ; l++)
            {
                string line = lines[l];
                for (int i = 0 ; i < line.Length ; i ++)
                {
                    if (Char.GetNumericValue(line[i]) >= 0)
                    {
                        BallConnected newBall = Instantiate(ballPref, GetOffsetByGridPos(new Vector2Int(i, l)), Quaternion.identity, startSpawn);
                        newBall.name = i + "," + l;
                        newBall.SetOnGrid(i, l);
                        newBall.SetParents(FindParents(i, l));
                        newBall.ChangeType(loadBallTypes[Int32.Parse(line[i].ToString())]);
                        allBalls.Add(newBall);
                    }
                }
            }

            foreach (var ball in allBalls)
            {
                ball.SetNeighboors(FindNeighboors(ball.GridPos));
            }
            
            return allBalls;
        }

        public void StartPopSequence(BallConnected startBall)
        {
            //находим все шары, которые надо лопнуть
            //лопаем шары
            //пересчитываем какие шары еще подвешены
            
            List<BallConnected> ballsToPop = new List<BallConnected>();
            ballsToPop.AddRange(GetConnectedBlock(startBall, null));
            ballsToPop.Add(startBall);
            Debug.Log("balls to pop " + ballsToPop.Count);
            foreach (var ball in ballsToPop)
            {
                if (allBalls.Contains(ball))
                {
                    ball.PopBall();
                }
            }
        }

        public void RemoveBall(BallConnected theBall)
        {
            allBalls.Remove(theBall);
        }

        public void ConnectBall(BallConnected toBall, Vector2 atPosition, BallType type)
        {
            Vector2 ballPosition = toBall.transform.position;
            float hitAngle = Vector2.SignedAngle(Vector2.up, (ballPosition - atPosition).normalized);
            Vector2Int gridPos = GetNeighboorGridCellByAngle(toBall.GridPos, hitAngle);
            BallConnected newBall = Instantiate(ballPref, GetOffsetByGridPos(gridPos), 
                Quaternion.identity, startSpawn);
            newBall.ChangeType(type);
            Debug.Log(gridPos);
            newBall.SetOnGrid(gridPos.x, gridPos.y);
            newBall.SetNeighboors(FindNeighboors(gridPos));
            allBalls.Add(newBall);

            foreach (var neighboor in newBall.Neighboor)
            {
                neighboor.AddNeighboor(newBall);
                
                if (neighboor.Type == newBall.Type)
                {
                    StartPopSequence(newBall);
                }
            }
        }

        private Vector2Int GetNeighboorGridCellByAngle(Vector2Int gridCell, float angle)
        {
            if (angle >= 0f && angle < 60f)
            {
                return gridCell + new Vector2Int(gridCell.y % 2 == 0 ? 0 : 1 , 1);
            } else if (angle >= 60f && angle < 120f)
            {
                return gridCell + new Vector2Int(1, 0);
            } else if (angle >= 120f && angle < 180f)
            {
                return gridCell + new Vector2Int(gridCell.y % 2 == 0 ? 0 : 1, -1);
            } else if (angle >= -180f && angle < -120f)
            {
                return gridCell + new Vector2Int(gridCell.y % 2 == 0 ? -1 : 0, -1);
            } else if (angle >= -120f && angle < -60f)
            {
                return gridCell + new Vector2Int(-1, 0);
            }
            else
            {
                return gridCell + new Vector2Int(gridCell.y % 2 == 0 ? -1 : 0, 1);
            }
        }

        private Vector2 GetOffsetByGridPos(Vector2Int gridCell)
        {
            Vector2 offset = startSpawn.position +
                             new Vector3(gridCell.x * xOffset + gridCell.y % 2 * divOffset,
                                 -gridCell.y * yOffset);
            return offset;
        }

        private List<BallConnected> GetConnectedBlock(BallConnected startBall, List<BallConnected> checkedBalls)
        {
            if (checkedBalls == null)
            {
                checkedBalls = new List<BallConnected>();
            }
            List<BallConnected> connectedBalls = new List<BallConnected>();
            
            checkedBalls.Add(startBall);
            
            foreach (var checkBall in startBall.Neighboor)
            {
                if (checkedBalls.Contains(checkBall) || connectedBalls.Contains(checkBall))
                {
                    break;
                }
                if (checkBall.Type == startBall.Type)
                {
                    connectedBalls.Add(checkBall);
                    connectedBalls.AddRange(GetConnectedBlock(checkBall, checkedBalls));
                }
                else
                {
                    checkedBalls.Add(checkBall);
                }
            }

            return connectedBalls;
        }

        private List<BallConnected> FindNeighboors(Vector2Int gridPos)
        {
            List<BallConnected> neighboors = new List<BallConnected>();
            
            foreach (var ball in allBalls)
            {
                if ((ball.GridPos == gridPos + Vector2Int.left) ||
                    (ball.GridPos == gridPos + Vector2Int.right) ||
                    (ball.GridPos == gridPos + Vector2Int.up) ||
                    (ball.GridPos == gridPos + Vector2Int.down) ||
                    (ball.GridPos == gridPos + new Vector2Int((gridPos.y % 2) == 0 ? -1 : 1, - 1)) ||
                    (ball.GridPos == gridPos + new Vector2Int((gridPos.y % 2) == 0 ? -1 : 1, 1)))
                {
                    Debug.Log("neighbor with:" + ball.GridPos);
                    neighboors.Add(ball);
                }
            }
            return neighboors;
        }

        private List<BallConnected> FindParents(int x, int y)
        {
            List<BallConnected> parents = new List<BallConnected>();
            if (y <= 0)
            {
                return parents;
            }   
            foreach (var ball in allBalls)
            {
                if (ball.GridPos == new Vector2Int(x, y - 1) || ball.GridPos == new Vector2Int(x + ((x % 2) == 0? - 1 : 1), y - 1))
                {
                    parents.Add(ball);
                }
            }
            return parents;
        }
    }
}