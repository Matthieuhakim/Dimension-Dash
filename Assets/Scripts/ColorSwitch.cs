using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSwitch : DimensionSwitcher
{
    [SerializeField]
    private Color realityColor;
    [SerializeField]
    private Color shadowColor;

    private SpriteRenderer spriteRenderer;



    public override void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        base.Start();

    }


    public override void SwitchDimension(Dimension dimension)
    {
        switch (dimension)
        {
            case Dimension.REALITY:
                spriteRenderer.color = realityColor;
                return;

            case Dimension.SHADOW:
                spriteRenderer.color = shadowColor;
                return;

            default:
                throw new System.Exception("Dimension: " + dimension +" not implemented for " + gameObject.name);

        }
    }





}
