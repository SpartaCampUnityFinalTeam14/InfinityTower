using UnityEngine;

[CreateAssetMenu(fileName = "MonsterSpriteSet", menuName = "Monster/SpriteSet")]
public class MonsterSpriteSetSO : ScriptableObject
{
    public Sprite[] walkDown;
    public Sprite[] walkLeft;
    public Sprite[] walkRight;
    public Sprite[] walkUp;

    public Sprite[] death;
}