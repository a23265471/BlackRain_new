using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputKey
{
    [System.Serializable]
    public class MoveKey
    {
        public KeyCode Forward;
        public KeyCode Back;
        public KeyCode Right;
        public KeyCode Left;
    }

    public MoveKey moveKey;
    public KeyCode Avoid;
    public KeyCode Dash;
    
    public KeyCode NormalAttack;
    public KeyCode Aim_Shoot;

}
