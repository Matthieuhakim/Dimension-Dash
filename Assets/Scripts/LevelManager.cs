using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public static LevelManager Instance { get; private set; }

    private Dimension currentDimension = Dimension.REALITY;

    private DimensionSwitcher[] levelObjects;

    public void Awake()
    {
        Instance = this;
        levelObjects = FindObjectsByType<DimensionSwitcher>();

    }

    public Dimension GetCurrentDimension()
    {
        return currentDimension;
    }

    public void SwitchDimension()
    {
        if(currentDimension == Dimension.REALITY)
        {
            currentDimension = Dimension.SHADOW;
        }
        else
        {
            currentDimension = Dimension.REALITY;
        }

        foreach(DimensionSwitcher levelObject in levelObjects)
        {
            levelObject.SwitchDimension(currentDimension);
        }
    }

}
