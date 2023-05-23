using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DimensionSwitcher : MonoBehaviour
{

    public abstract void SwitchDimension(Dimension dimension);

    public virtual void Start()
    {
        Dimension curDimension = LevelManager.Instance.GetCurrentDimension();
        SwitchDimension(curDimension);
    }
}

public enum Dimension { REALITY, SHADOW };
