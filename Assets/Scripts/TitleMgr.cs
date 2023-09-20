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

    public GameObject m_DescriptionPanel = null;
    public Button m_ExitBtn = null;

    // Start is called before the first frame update
    void Start()
    {
        if (m_StartGameBtn != null)
            m_StartGameBtn.onClick.AddListener(() =>
            {

                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameUIScene", LoadSceneMode.Additive);
                
            });

        if (m_GameDescriptionBtn != null)
            m_GameDescriptionBtn.onClick.AddListener(() =>
            {
                if (m_DescriptionPanel != null && m_DescriptionPanel.activeSelf == false)
                    m_DescriptionPanel.SetActive(true);

                if (m_ExitBtn != null)
                    m_ExitBtn.onClick.AddListener(() =>
                    {
                        m_DescriptionPanel.SetActive(false);
                    });

            });

        if (m_GameEndBtn != null)
            m_GameEndBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        Sound_Mgr.Instance.PlayBGM("under the rainbow", 1.0f);

    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
