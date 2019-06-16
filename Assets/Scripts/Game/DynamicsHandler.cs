using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicsHandler : MonoBehaviour
{
    internal abstract bool ChangeLetter(int inTextPosition, int code);
    internal abstract bool AddLetter(int inTextPosition, int code);
    internal abstract bool RemoveLetter(int inTextPosition);
    internal abstract string GetLastWord();
}