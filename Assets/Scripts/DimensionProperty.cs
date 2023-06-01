using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DimensionProperty : ColorSwitch
{

    [SerializeField]
    protected Color bothColor;

    [SerializeField]
    protected Dimension preferedDimension;

    private float disabledAlpha = 0.4f;


    private Collider2D spriteCollider;


    // Start is called before the first frame update
    public override void Start()
    {
        spriteCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        //Take color of prefered dimension at start
        TakePreferredColor();

        SwitchDimension(LevelManager.Instance.GetCurrentDimension());
    }


    //Activate or deactivate object if it is dimension dependant
    public override void SwitchDimension(Dimension dimension)
    {
        if(preferedDimension != Dimension.BOTH)
        {

            if(preferedDimension == dimension)
            {
                ActivateObject();
            }
            else
            {
                DeactivateObject();
            }

        }
    }

    //Properties when in prefered dimension
    protected virtual void ActivateObject()
    {
        spriteCollider.enabled = true;
        spriteRenderer.color = ChangeAlpha(spriteRenderer.color, 1f);
    }

    //Properties when not in prefered dimension
    protected virtual void DeactivateObject()
    {
        spriteCollider.enabled = false;
        spriteRenderer.color = ChangeAlpha(spriteRenderer.color, disabledAlpha);

    }

    private void TakePreferredColor()
    {
        if (preferedDimension == Dimension.BOTH)
        {
            spriteRenderer.color = bothColor;
        }
        else
        {
            base.SwitchDimension(preferedDimension);
        }
    }


    private Color ChangeAlpha(Color color, float a)
    {
        return new Color(color.r, color.g, color.b, a);
    }


}
