using UnityEngine;


[CreateAssetMenu(fileName = "Ball", menuName = "Ball", order = 0)]
public class BallType : ScriptableObject
{
    [SerializeField]
    private Sprite sprite;
    [SerializeField]
    private BallColor color;

    [SerializeField] 
    private int score;
    public Sprite BallSprite => sprite;

    public BallColor Color => color;

    public int Score => score;
}

public enum BallColor
{
    Red,
    Blue,
    Yellow,
    Green
}
