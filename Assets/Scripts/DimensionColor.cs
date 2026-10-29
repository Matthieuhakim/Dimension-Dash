using UnityEngine;

// Changes colour with the world's dimension (player, floor, background). Never changes collision.
public class DimensionColor : DimensionListener
{
    [SerializeField]
    private Color realityColor = Color.white;
    [SerializeField]
    private Color shadowColor = Color.white;

    public Color CurrentColor => Tint;

    protected override void OnDimensionChanged(Dimension dimension)
    {
        Tint = dimension == Dimension.Reality ? realityColor : shadowColor;
    }
}
