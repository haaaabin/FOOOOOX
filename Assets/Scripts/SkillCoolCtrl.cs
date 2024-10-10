using UnityEngine;
using UnityEngine.UI;

public class SkillCoolCtrl : MonoBehaviour
{
    [HideInInspector] public SkillType m_SkType;
    public Image Time_Image = null;
    public Image Icon_Image = null;
    private float skill_Time = 0.0f;
    private float skill_Delay = 0.0f;

    private void Update()
    {
        skill_Time -= Time.deltaTime;
        Time_Image.fillAmount = skill_Time / skill_Delay;

        if (skill_Time <= 0.0f)
            Destroy(this.gameObject);
    }

    public void InitState(float a_Time, float a_Delay)
    {
        skill_Time = a_Time;
        skill_Delay = a_Delay;
    }
}
