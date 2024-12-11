using IterationToolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : GlobalManager
{
    public static new GameManager Instance => SingletonManager.GetSingleton<GameManager>(typeof(GameManager));
}
