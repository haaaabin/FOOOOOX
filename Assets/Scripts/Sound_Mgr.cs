using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioNode : MonoBehaviour
{
    [HideInInspector] public AudioSource m_AudioSrc = null;     // 각 레이어별 AudioSource 컴포넌트를 저장하기 위한 변수
    [HideInInspector] public float m_EffVolume = 0.2f;           // 각 레이어별 사운드 볼륨
    [HideInInspector] public float m_PlayTime = 0.0f;           // 각 레이어별 타이머

    void Update()
    {
        if (0.0f < m_PlayTime)
            m_PlayTime -= Time.deltaTime;
    }
}

public class Sound_Mgr : G_Singleton<Sound_Mgr>
{
    [HideInInspector] public AudioSource m_AudioSrc = null;
    Dictionary<string, AudioClip> m_ADClipList = new Dictionary<string, AudioClip>();   //Dictonary로 클립 파일명 저장

    //--- 효과음 최적화를 위한 버퍼 변수
    int m_EffSdCount = 20;      // 지금은 20개의 레이어로 플레이...
    List<AudioNode> m_AudNodeList = new List<AudioNode>();
    //--- 효과음 최적화를 위한 버퍼 변수

    float m_bgmVolume = 0.2f;
    [HideInInspector] public bool m_SoundOnOff = true;
    [HideInInspector] public float m_SoundVolume = 1.0f;

    protected override void Init()  //Awake() 함수 대신 사용
    {
        base.Init();    // 부모쪽에 있는 Init() 함수 호출

        LoadChildGameObj();
    }

    // Start is called before the first frame update
    void Start()
    {
        // --- 사운드 미리 로딩
        AudioClip a_GAudioiClip = null;
        object[] temp = Resources.LoadAll("Sounds");
        for (int ii = 0; ii < temp.Length; ii++)
        {
            a_GAudioiClip = temp[ii] as AudioClip;  // a_GAudioiClip에 사운드 로딩

            if (m_ADClipList.ContainsKey(a_GAudioiClip.name) == true)   //이미 저장되어있으면 넘겨
                continue;

            m_ADClipList.Add(a_GAudioiClip.name, a_GAudioiClip);    //m_ADClipList에 파일이름, 객체 저장
        }
        // --- 사운드 미리 로딩
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadChildGameObj()  //사운드 로딩
    {
        m_AudioSrc = this.gameObject.AddComponent<AudioSource>();   //스크립트상 AudioSource 컴포넌트 추가

        for (int ii = 0; ii < m_EffSdCount; ii++)    //20개 생성
        {
            GameObject newSoundObj = new GameObject("SoundEffObj");     //빈 게임오브젝트 생성
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            AudioNode a_AudioNode = newSoundObj.AddComponent<AudioNode>();
            a_AudioNode.m_AudioSrc = a_AudioSrc;
            m_AudNodeList.Add(a_AudioNode);     //리스트로 관리

        } //for(int ii = 0; ii < m_EffSdCount; ii++)

        // --- 사운드 OnOff, 사운드 볼륨 로컬 로딩 후 적용
        int a_SoundOnOff = PlayerPrefs.GetInt("SoundOnOff", 1);
        if (a_SoundOnOff == 1)
            SoundOnOff(true);
        else
            SoundOnOff(false);

        float a_Value = PlayerPrefs.GetFloat("SoundVolume", 1.0f);
        SoundVolume(a_Value);
        // --- 사운드 OnOff, 사운드 볼륨 로컬 로딩 후 적용

    }

    public void PlayBGM(string a_FileName, float fVolume = 0.2f)
    {
        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;     //존재하지 않으면 다시 로딩
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
            return;

        if (m_AudioSrc.clip != null && m_AudioSrc.clip.name == a_FileName) //비어있지 않거나 같은 이름의 파일이 존재하면 그냥 넘겨
            return;

        m_AudioSrc.clip = a_GAudioClip;
        m_AudioSrc.volume = fVolume * m_SoundVolume;
        m_bgmVolume = fVolume;
        m_AudioSrc.loop = true;     //무한루프
        m_AudioSrc.Play();          //연결되어있는 오디오 하나만 플레이

    } //public void PlayBGM(string a_FileName, float fVolume = 0.2f)

    public void PlayGUISound(string a_FileName, float fVolume = 0.2f)
    {   //GUI 효과음 플레이 하기 위한 함수
        if (m_SoundOnOff == false)
            return;

        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (m_AudioSrc == null)
            return;

        m_AudioSrc.PlayOneShot(a_GAudioClip, fVolume * m_SoundVolume);  //중복음 플레이 가능

    } //public void PlayGUISound(string a_FileName, float fVolume = 0.2f)

    public void PlayEffSound(string a_FileName, float fVolume = 0.2f)
    {
        if (m_SoundOnOff == false)
            return;

        AudioClip a_GAudioClip = null;
        if (m_ADClipList.ContainsKey(a_FileName) == true)
        {
            a_GAudioClip = m_ADClipList[a_FileName] as AudioClip;
        }
        else
        {
            a_GAudioClip = Resources.Load("Sounds/" + a_FileName) as AudioClip;
            m_ADClipList.Add(a_FileName, a_GAudioClip);
        }

        if (a_GAudioClip == null)
            return;

        bool isPlayOK = false;

        AudioSource a_AudSrc = null;
        foreach (AudioNode a_AudNode in m_AudNodeList)
        {
            if (a_AudNode == null)
                continue;

            //이전 사운드를 아직 플레이 중이면 스킵
            if (0.0f < a_AudNode.m_PlayTime)
                continue;
            //플레이를 쉬고 있는 AudioSource만 재활용 한다.

            a_AudSrc = a_AudNode.m_AudioSrc;

            a_AudSrc.volume = fVolume * m_SoundVolume;  //m_SoundVolume 변수로 게임의 전체적인 볼륨 설정 가능
            a_AudSrc.clip = a_GAudioClip;
            a_AudNode.m_EffVolume = fVolume;
            a_AudNode.m_PlayTime = a_GAudioClip.length + 0.7f;  // 사운드 플레이 시간
            a_AudSrc.Play();

            isPlayOK = true;
            break;

        } //foreach(AudioNode a_AudNode in m_AudNodeList)

        if (isPlayOK == false)   // 사운드 추가 필요
        {
            GameObject newSoundObj = new GameObject("SoundEffObj");     //빈 게임오브젝트 생성
            newSoundObj.transform.SetParent(this.transform);
            newSoundObj.transform.localPosition = Vector3.zero;
            AudioSource a_AudioSrc = newSoundObj.AddComponent<AudioSource>();
            a_AudioSrc.playOnAwake = false;
            a_AudioSrc.loop = false;
            AudioNode a_AudioNode = newSoundObj.AddComponent<AudioNode>();
            a_AudioNode.m_AudioSrc = a_AudioSrc;
            m_AudNodeList.Add(a_AudioNode);     //리스트로 관리

            // --- 사운드 플레이
            a_AudioSrc.volume = fVolume * m_SoundVolume;  //m_SoundVolume 변수로 게임의 전체적인 볼륨 설정 가능
            a_AudioSrc.clip = a_GAudioClip;
            a_AudioNode.m_EffVolume = fVolume;
            a_AudioNode.m_PlayTime = a_GAudioClip.length + 0.7f;  // 사운드 플레이 시간
            a_AudioSrc.Play();

        } //if(isPlayOK == false)   // 사운드 추가 필요

    } //public void PlayEffSound(string a_FileName, float fVolume = 0.2f)

    public void SoundOnOff(bool a_OnOff = true)
    {
        bool a_MuteOnOff = !a_OnOff;    //반전시켜서 가져옴

        if (m_AudioSrc != null)
        {
            m_AudioSrc.mute = a_MuteOnOff;  // mute == true 끄기, mute == false 켜기
            if (a_MuteOnOff == false)   // 사운드를 다시 켰을 때
                m_AudioSrc.time = 0;    // 처음부터 다시 플레이
        }

        foreach (AudioNode a_AudNode in m_AudNodeList)
        {
            if (a_AudNode == null)
                continue;

            a_AudNode.m_AudioSrc.mute = a_MuteOnOff;
            if (a_MuteOnOff == false)   // 사운드를 다시 켰을 때
                a_AudNode.m_AudioSrc.time = 0;    // 처음부터 다시 플레이
        }

        m_SoundOnOff = a_OnOff;
    }

    public void SoundVolume(float fVolume)
    {
        if (m_AudioSrc != null)
            m_AudioSrc.volume = m_bgmVolume * fVolume;

        foreach (AudioNode a_AudNode in m_AudNodeList)
        {
            if (a_AudNode == null)
                continue;

            a_AudNode.m_AudioSrc.volume = a_AudNode.m_EffVolume * fVolume;
        }

        m_SoundVolume = fVolume;    //전체 볼륨 저장

    } //public void SoundVolume(float fVolume)

}

