using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorTarget : MonoBehaviour
{
    [field: SerializeField] public Texture2D CursorIcon { get; private set; }

    private void OnMouseOver()
    {
        GameManager.Player.RequestNewCursor(CursorIcon);
    }
}
