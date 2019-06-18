using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DynamicsFlag
{
    internal static readonly DynamicsFlag DF_FULL;
    internal static readonly DynamicsFlag DF_ONLY_CHANGE;
    internal static readonly DynamicsFlag DF_CHANGE_ADD;

    static DynamicsFlag()
    {
        DF_FULL = new DynamicsFlag(true, true, true);

        DF_CHANGE_ADD = new DynamicsFlag(true, true);

        DF_ONLY_CHANGE = new DynamicsFlag(true);
    }

    public DynamicsFlag(bool change, bool add = false, bool remove = false)
    {
        canChangeLetter = change;
        canAddLetter = add;
        canRemoveLetter = remove;
    }

    public bool canAddLetter;
    public bool canRemoveLetter;
    public bool canChangeLetter;
}