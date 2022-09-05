using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object", fileName = "Scriptable Object / Characters")]
public class CharacterScriptableObject : ScriptableObject
{
    public int id;

    public Sprite characterSprite;
    public CharacterRank characterRank;
    
    public enum CharacterRank
    {
        E = 0,
        D = 1,
        C = 2,
        B = 3,
        A = 4,
        X = 5,
        S = 6
    }
}
