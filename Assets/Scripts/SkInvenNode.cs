using UnityEngine;
using UnityEngine.UI;

public class SkInvenNode : MonoBehaviour
{
    [HideInInspector] public SkillType skType;
    [HideInInspector] public Text skCountText;

    private void Awake()
    {
        skCountText = GetComponentInChildren<Text>();
    }
}
