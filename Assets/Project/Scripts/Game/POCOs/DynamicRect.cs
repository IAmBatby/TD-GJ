using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicRect
{
    public Rect DefaultRect { get; private set; }
    public Rect LastRect { get; private set; }

    public Vector2 Position { get; private set; }
    public Vector2 Size { get; private set; }
    public Vector2 Additive { get; private set; }
    public Vector2 Spacing { get; private set; }
    public Vector2 Scaling { get; private set; }

    public DynamicRect(Rect newDefault, float scalingX = 0f, float scalingY = 0f, float spacingX = 0f, float spacingY = 0f, float defaultWidth = -1f, float defaultHeight = -1f)
    {
        Apply(newDefault, scalingX, scalingY, spacingX, spacingY, defaultWidth, defaultHeight);
    }

    public DynamicRect(float x, float y, float width, float height, float scalingX = 0f, float scalingY = 0f, float spacingX = 0f, float spacingY = 0f, float defaultWidth = -1f, float defaultHeight = -1f)
    {
        Apply(new Rect(x,y,width, height), scalingX, scalingY, spacingX, spacingY, defaultWidth, defaultHeight);
    }

    private void Apply(Rect newDefaultRect, float scalingX = 0f, float scalingY = 0f, float spacingX = 0f, float spacingY = 0f, float defaultWidth = -1f, float defaultHeight = -1f)
    {
        DefaultRect = newDefaultRect;
        LastRect = DefaultRect;
        Position = new Vector2(newDefaultRect.x, newDefaultRect.y);
        Size = new Vector2(newDefaultRect.width, newDefaultRect.height);
        Scaling = new Vector2(scalingX, scalingY);
        Spacing = new Vector2(spacingX, spacingY);
        if (defaultWidth > -1f)
            Size = new Vector2(defaultWidth, Size.y);
        if (defaultHeight > -1f)
            Size = new Vector2(Size.x, defaultHeight);
    }

    public void Modify(float x = 0f, float y = 0f, float w = 0f, float h = 0f, float addX = 0f, float addY = 0f, float scaleX = 0f, float scaleY = 0f, float spaceX = 0f, float spaceY = 0f)
    {
        if (x != 0f) Position = new Vector2(Position.x + x, Position.y);
        if (y != 0f) Position = new Vector2(Position.x, Position.y + y);
        if (w != 0f) Size = new Vector2(Size.x + w, Size.y);
        if (h != 0f) Size = new Vector2(Size.x, Size.y + h);
        if (addX != 0f) Additive = new Vector2(Additive.x + addX, Additive.y);
        if (addY != 0f) Additive = new Vector2(Additive.x, Additive.y + addY);
        if (scaleX != 0f) Scaling = new Vector2(Scaling.x + scaleX, Scaling.y);
        if (scaleY != 0f) Scaling = new Vector2(Scaling.x, Scaling.y + scaleY);
        if (spaceX != 0f) Spacing = new Vector2(Spacing.x + spaceX, Spacing.y);
        if (spaceY != 0f) Spacing = new Vector2(Spacing.x, Spacing.y + spaceY);
    }

    public void Set(float x = 0f, float y = 0f, float w = 0f, float h = 0f, float addX = 0f, float addY = 0f, float scaleX = 0f, float scaleY = 0f, float spaceX = 0f, float spaceY = 0f)
    {
        if (x != 0f) Position = new Vector2(x, Position.y);
        if (y != 0f) Position = new Vector2(Position.x, y);
        if (w != 0f) Size = new Vector2(w, Size.y);
        if (h != 0f) Size = new Vector2(Size.x, h);
        if (addX != 0f) Additive = new Vector2(addX, Additive.y);
        if (addY != 0f) Additive = new Vector2(Additive.x, addY);
        if (scaleX != 0f) Scaling = new Vector2(scaleX, Scaling.y);
        if (scaleY != 0f) Scaling = new Vector2(Scaling.x, scaleY);
        if (spaceX != 0f) Spacing = new Vector2(spaceX, Spacing.y);
        if (spaceY != 0f) Spacing = new Vector2(Spacing.x, spaceY);
    }

    public Rect Get(float w = -1f, float h = -1f, float x = 0f, float y = 0f)
    {
        if (w < 0f)
            w = Size.x;
        if (h < 0f)
            h = Size.y;
        Additive += new Vector2(x, y);
        Rect returnRect = new Rect((Position.x + Additive.x), Position.y + Additive.y, w, h);
        Additive += new Vector2(Scaling.x * (w + Spacing.x), Scaling.y * (h + Spacing.y));
        LastRect = returnRect;
        return (returnRect);
    }
}
