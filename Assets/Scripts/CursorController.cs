using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{
    //instance of the CursorController - there should only be one present in the game at any given time
    public static CursorController instance;

    //inspector settings
    public Texture2D crossHairCursor, navigationCursor;

    private void Awake() // called before Start 
    {
        instance = this; // initializing the instance;
    }

    public void SetToCrossHairCursor()
    {
        // Changing the hotspot/clicking position of the cursor to be the center of the cursor image
        Cursor.SetCursor(crossHairCursor, new Vector2(crossHairCursor.width/2, crossHairCursor.height/2), CursorMode.Auto);
    }

    public void SetToNavigationCursor()
    {
        // Vector2.zero doesnt change the hotspot of the mouse cursor
        // it stays at the default which is the top left of the image
        Cursor.SetCursor(navigationCursor, Vector2.zero, CursorMode.Auto);
    }
}
