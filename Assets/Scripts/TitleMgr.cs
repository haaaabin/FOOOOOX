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
                SceneManager.LoadScene("Level1");
                SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
            });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
