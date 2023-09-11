using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Fade_Mgr : MonoBehaviour
{
    public bool IsFadeOut = false;  //나갈때 
    public bool IsFadeIn = false;   //이 씬으로 들어올 때 

    //--- Fade In Out 관련 변수들...
    Image m_FadeImg = null;
    float AniDuring = 0.8f;     //페이드아웃 연출 시간 설정
    bool m_StartFade = false;
    float m_CacTime = 0.0f;
    float m_AddTimer = 0.0f;
    Color m_Color;

    float m_StVal = 1.0f;
    float m_EndVal = 0.0f;

    string m_SceneName = "";    //이동할 씬 이름 저장용 변수
    string m_SceneName2 = "";
    //--- Fade In Out 관련 변수들...

    public static Fade_Mgr Inst = null;

    private void Awake()
    {
        Inst = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject a_Canvas = GameObject.Find("Canvas");
        if (a_Canvas != null)
        {
            //매개변수 true의 의미는 Active가 꺼져 있는 게임오브젝트도 모두 가져오라는 뜻
            Image[] a_ImgList = a_Canvas.transform.GetComponentsInChildren<Image>(true);
            for (int ii = 0; ii < a_ImgList.Length; ii++)
            {
                if (a_ImgList[ii].gameObject.name == "FadePanel")
                {
                    m_FadeImg = a_ImgList[ii];
                    break;
                }
            }
        }//if(a_Canvas != null)

        //--- Fade In 초기화
        if (m_FadeImg != null && IsFadeIn == true)
        {
            m_StVal = 1.0f;
            m_EndVal = 0.0f;
            m_FadeImg.color = new Color32(0, 0, 0, 255);
            m_FadeImg.gameObject.SetActive(true);
            m_StartFade = true;
        }
        //--- Fade In 초기화

    }//void Start()

    // Update is called once per frame
    void Update()
    {
        if (m_FadeImg == null)
            return;

        FadeUpdate();
    }

    void FadeUpdate()
    {
        if (m_StartFade == false)
            return;

        if (m_CacTime < 1.0f)
        {
            m_AddTimer += Time.deltaTime;
            m_CacTime = m_AddTimer / AniDuring;
            m_Color = m_FadeImg.color;
            m_Color.a = Mathf.Lerp(m_StVal, m_EndVal, m_CacTime);
            m_FadeImg.color = m_Color;

            if (1.0f <= m_CacTime)
            {
                if (m_StVal == 1.0f && m_EndVal == 0.0f) //들어올 때 
                {
                    m_Color.a = 0.0f;
                    m_FadeImg.color = m_Color;
                    m_FadeImg.gameObject.SetActive(false);
                    m_StartFade = false;
                }
                else if (m_StVal == 0.0f && m_EndVal == 1.0f) //나갈 때 
                {
                    SceneManager.LoadScene(m_SceneName);
                    SceneManager.LoadScene(m_SceneName2, LoadSceneMode.Additive);
                }
            }//if(1.0f <= m_CacTime)

        }//if (m_CacTime < 1.0f)
    }//void FadeUpdate()

    public void SceneOut(string a_ScName)
    {
        if (m_FadeImg == null)
            return;

        m_SceneName = a_ScName;

        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.color = new Color32(0, 0, 0, 0);
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }//public void SceneOut(string a_ScName)

    public void SceneOuts(string a_ScName, string a_ScName2)
    {
        if (m_FadeImg == null)
            return;

        m_SceneName = a_ScName;
        m_SceneName2 = a_ScName2;

        m_CacTime = 0.0f;
        m_AddTimer = 0.0f;
        m_StVal = 0.0f;
        m_EndVal = 1.0f;
        m_FadeImg.color = new Color32(0, 0, 0, 0);
        m_FadeImg.gameObject.SetActive(true);
        m_StartFade = true;
    }//public void SceneOut(string a_ScName)
}