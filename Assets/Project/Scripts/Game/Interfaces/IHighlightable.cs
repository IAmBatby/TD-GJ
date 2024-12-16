using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHighlightable
{
    public List<ContentDisplayListing> GetDisplayListings();

    public Texture2D GetCursor();

    public bool IsHighlightable();

    public bool Compare(GameObject go);
}
