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

}
