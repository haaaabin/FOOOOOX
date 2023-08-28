using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public Button m_StartGameBtn = null;
    public Button m_GameDescriptionBtn = null;
    public Button m_GameEndBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_StartGameBtn != null)
            m_StartGameBtn.onClick.AddListener(()=>
            {
                if (Fade_Mgr.Inst != null && Fade_Mgr.Inst.IsFadeOut == true)
                    Fade_Mgr.Inst.SceneOut("Level1");

                else
                    SceneManager.LoadScene("Level1");
            });

        if (m_GameEndBtn != null)
            m_GameEndBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
