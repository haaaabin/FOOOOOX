using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillType
{
    Skill_0 = 0,
    Skill_1,
    SkCount
}

public class GlobalValue
{
    public static int g_UserGold = 0;
    public static float g_Hp = 500;

    public static int[] g_skillCount = new int[2];

    public static void LoadGameData()
    {
        g_UserGold = PlayerPrefs.GetInt("UserGold", 0);
        g_Hp = PlayerPrefs.GetFloat("Hp", 500);
        string a_Skill = "";
        for (int i = 0; i < g_skillCount.Length; i++)
        {
            a_Skill = "SkItem_" + i.ToString();
            g_skillCount[i] = PlayerPrefs.GetInt("SkillCount", 0);
        }

        PlayerPrefs.SetFloat("Hp", 500);
        PlayerPrefs.SetInt("UserGold", 0);
    }
}
