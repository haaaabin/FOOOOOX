using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public static int g_UserGold = 0;
    public static int g_Hp = 500;

    public static void LoadGameData()
    {
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
        g_Hp = PlayerPrefs.GetInt("Hp", 500);
    }
}
