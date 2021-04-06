using System;
using System.Collections.Generic;
using System.Linq;
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
        [SerializeField] private string levelName = "Level1";
        
        private Dictionary<Vector2Int, BallConnected> allBalls;
        private double minHangBalls = 0;
        private GameController _gameController;
        
        private void Start()
        {
            _gameController = GameController.instance;
            GenerateBalls();
        }

        /// <summary>
        /// Создать поле шаров
        /// </summary>
        /// <returns></returns>
        public Dictionary<Vector2Int, BallConnected> GenerateBalls()
        {
            allBalls = new Dictionary<Vector2Int, BallConnected>();
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
                        Vector2Int gridPos = new Vector2Int(i, l);
                        newBall.SetOnGrid(gridPos);
                        if (l == 0)
                        {
                            newBall.HangUp();
                            minHangBalls++;
                        }
                        newBall.ChangeType(loadBallTypes[Int32.Parse(line[i].ToString())]);
                        allBalls.Add(gridPos, newBall);
                    }
                }
            }

            foreach (var ball in allBalls)
            {
                if (FindNeighboors(ball.Value.GridPos).Any(b => b.Hanging))
                {
                    ball.Value.HangUp();
                }
            }

            minHangBalls *= 0.3f;
            return allBalls;
        }

        /// <summary>
        /// запустить взрыв шаров одного и того же типа
        /// </summary>
        /// <param name="startBall"></param>
        public void StartPopSequence(BallConnected startBall)
        {
            HashSet<BallConnected> ballsToPop = new HashSet<BallConnected>();
            //находим все шары, которые надо лопнуть
            foreach (var ball in GetConnectedBlockSameType(startBall, null))
            {
                ballsToPop.Add(ball);
            }

            if (ballsToPop.Count > 1)
            {
                //лопаем шары
                foreach (var ball in ballsToPop)
                {
                    if (allBalls.Values.Contains(ball))
                    {
                        ball.PopBall();
                        RemoveBall(ball);
                    }
                }
                DropUnHangBalls();
            }
            else
            {
                startBall.HangUp();
            }
        }

        private void RemoveBall(BallConnected theBall)
        {
            allBalls.Remove(theBall.GridPos);
        }
        
        /// <summary>
        /// Пробить шар насквозь и занять его место
        /// </summary>
        /// <param name="ballConnected"></param>
        public void SmackBall(BallConnected ballConnected, BallType newType)
        {
            ballConnected.PopBall();
            RemoveBall(ballConnected);
            
            BallConnected newBall = Instantiate(ballPref, GetOffsetByGridPos(ballConnected.GridPos), 
                Quaternion.identity, startSpawn);
            newBall.ChangeType(newType);
            newBall.SetOnGrid(ballConnected.GridPos);
            allBalls.Add(ballConnected.GridPos, newBall);

            StartPopSequence(newBall);
            _gameController.LoadBall();
        }
 
        /// <summary>
        /// Уронить все не подвешенные шары
        /// </summary>
        private void DropUnHangBalls()
        {
            List<BallConnected> fallOffBalls = new List<BallConnected>(allBalls.Values);
            int curHangBalls = 0;

            //пересчитываем какие шары еще подвешены
            foreach (var keys in allBalls.Keys.Where(d => d.y == 0).ToList())
            {
                curHangBalls++;
                BallConnected ball;
                if (allBalls.TryGetValue(keys, out ball))
                {
                    foreach (var hangBall in GetConnectedBlock(ball, new List<BallConnected>()))
                    {
                        hangBall.HangUp();
                        fallOffBalls.Remove(hangBall);
                    }
                }
            }
            
            //Роняем не подвешенные шары
            foreach (var ball in fallOffBalls)
            {
                ball.FallOff();
                RemoveBall(ball);
            }

            if (curHangBalls < minHangBalls)
            {
                _gameController.Win();
                foreach (var ball in allBalls)
                {
                    ball.Value.FallOff();
                }
            }
        }

        /// <summary>
        /// поставить шар на поле
        /// </summary>
        /// <param name="toBall"></param>
        /// <param name="atPosition"></param>
        /// <param name="type"></param>
        public void ConnectBall(BallConnected toBall, Vector2 atPosition, BallType type)
        {
            Vector2 ballPosition = toBall.transform.position;
            float hitAngle = Vector2.SignedAngle(Vector2.up, (ballPosition - atPosition).normalized);
            Vector2Int gridPos = GetNeighboorGridCellByAngle(toBall.GridPos, hitAngle);
            BallConnected newBall = Instantiate(ballPref, GetOffsetByGridPos(gridPos), 
                Quaternion.identity, startSpawn);
            newBall.ChangeType(type);
            newBall.SetOnGrid(gridPos);
            allBalls.Add(gridPos, newBall);
            StartPopSequence(newBall);
            _gameController.LoadBall();
        }

        /// <summary>
        /// Находим ближайшую ячейку, под углом
        /// </summary>
        /// <param name="gridCell">ячейка относительно которой будем смотреть</param>
        /// <param name="angle">угол падения на ячейку</param>
        /// <returns></returns>
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

        /// <summary>
        /// Позиция на игровом поле, относительно ячейки
        /// </summary>
        /// <param name="gridCell">ячейка</param>
        /// <returns></returns>
        private Vector2 GetOffsetByGridPos(Vector2Int gridCell)
        {
            Vector2 offset = startSpawn.position +
                             new Vector3(gridCell.x * xOffset + gridCell.y % 2 * divOffset,
                                 -gridCell.y * yOffset);
            return offset;
        }
        
        /// <summary>
        /// Все шары, до которых можно дойти относительно стартового. Вместе образуют единый блок.
        /// </summary>
        /// <param name="startBall">Начальный шар</param>
        /// <param name="checkedBalls"></param>
        /// <returns></returns>
        private HashSet<BallConnected> GetConnectedBlock(BallConnected startBall, List<BallConnected> checkedBalls)
        {
            if (checkedBalls == null)
            {
                checkedBalls = new List<BallConnected>();
            }
            var connected = new HashSet<BallConnected>();
            
            checkedBalls.Add(startBall);
            
            foreach (var checkBall in FindNeighboors(startBall.GridPos))
            {
                if (checkedBalls.Contains(checkBall))
                {
                    continue;
                }
                var subconnected = GetConnectedBlock(checkBall, checkedBalls);
                foreach (var ball in subconnected)
                {
                    connected.Add(ball);
                }
                
                checkedBalls.Add(checkBall);
            }

            connected.Add(startBall);

            return connected;
        }

        /// <summary>
        /// Все шары того же типа, что и стартовый, до которых можно дойти.
        /// </summary>
        /// <param name="startBall">Начальный шар</param>
        /// <param name="checkedBalls"></param>
        /// <returns></returns>
        private HashSet<BallConnected> GetConnectedBlockSameType(BallConnected startBall, List<BallConnected> checkedBalls)
        {
            if (checkedBalls == null)
            {
                checkedBalls = new List<BallConnected>();
            }
            var connected = new HashSet<BallConnected>();
            
            checkedBalls.Add(startBall);
            
            foreach (var checkBall in FindNeighboors(startBall.GridPos))
            {
                if (checkedBalls.Contains(checkBall))
                {
                    continue;
                }
                if (checkBall.Type == startBall.Type)
                {
                    var subconnected = GetConnectedBlockSameType(checkBall, checkedBalls);
                    foreach (var ball in subconnected)
                    {
                        connected.Add(ball);
                    }
                }
                checkedBalls.Add(checkBall);
            }

            connected.Add(startBall);

            return connected;
        }

        /// <summary>
        /// Найти всех существующих соседей у блока 
        /// </summary>
        /// <param name="gridPos"></param>
        /// <returns></returns>
        private List<BallConnected> FindNeighboors(Vector2Int gridPos)
        {
            List<BallConnected> neighboors = new List<BallConnected>();
            
            foreach (var ball in allBalls)
            {
                Vector2Int ballPos = ball.Key;
                if ((ballPos == gridPos + Vector2Int.left) ||
                    (ballPos == gridPos + Vector2Int.right) ||
                    (ballPos == gridPos + Vector2Int.up) ||
                    (ballPos == gridPos + Vector2Int.down) ||
                    (ballPos == gridPos + new Vector2Int((gridPos.y % 2) == 0 ? -1 : 1, - 1)) ||
                    (ballPos == gridPos + new Vector2Int((gridPos.y % 2) == 0 ? -1 : 1, 1)))
                {
                    neighboors.Add(ball.Value);
                }
            }
            return neighboors;
        }
    }
}