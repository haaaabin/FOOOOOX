using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public SkillType skillType = SkillType.SkCount;
   
    public enum GameState { Level1, Boss, GameOver, Ending }
    public GameState gameState;

    public static GameManager Instance()
    {
        if (instance == null)
        {
            Debug.LogWarning("GameManager 인스턴스가 존재하지 않습니다.");
            return null;
        }
        return instance;
    }

    private void Awake()
    {
        if (instance != null)
            Destroy(instance);
        instance = this;
    }

    private void Start()
    {
        Time.timeScale = 1.0f;

        if (SceneManager.GetActiveScene().name == "Level1")
        {
            SoundManager.Instance.PlayBGM("hurry_up_and_run", 1.0f);

        }
        else if (SceneManager.GetActiveScene().name == "BossScene")
        {
            SoundManager.Instance.PlayBGM("Maniac", 1.0f);
        }

        gameState = GameState.Level1;
    }


    private void Update()
    {
        if (!(gameState == GameState.GameOver))
        {
            // if (Input.GetKeyDown(KeyCode.Alpha1) ||
            //     Input.GetKeyDown(KeyCode.Keypad1))
            // {
            //     UseSkill_Key(SkillType.Skill_0);
            // }
            // else if (Input.GetKeyDown(KeyCode.Alpha2) ||
            //     Input.GetKeyDown(KeyCode.Keypad2))
            // {
            //     UseSkill_Key(SkillType.Skill_1);
            // }
        }
    }

    public void GameExit()
    {
        SceneManager.LoadScene("TitleScene");
    }

    // public void UseSkill_Key(SkillType skType)
    // {
    //     if (GlobalValue.g_skillCount[(int)skType] <= 0)
    //         return;

    //     PlayerCtrl.instance.UseSkill_Item(skType);

    //     if ((int)skType < skInvenNode.Length)
    //         skInvenNode[(int)skType].skCountText.text =
    //                     GlobalValue.g_skillCount[(int)skType].ToString();

    // }

}