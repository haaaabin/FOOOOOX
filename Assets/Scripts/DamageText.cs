using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public Text childText = null;
    private Vector3 curPos;
    private Color color;
    private float effectTime = 0.0f;
    private float mvVelocity = 1.1f / 1.05f;
    private float apVelocity = 1.0f / (1.0f - 0.4f);

    private void Update()
    {
        effectTime += Time.deltaTime;

        if (effectTime < 1.05f)
        {
            curPos = childText.transform.position;
            curPos.y += Time.deltaTime * mvVelocity;
            childText.transform.position = curPos;
        }

        if (0.4f < effectTime)
        {
            color = childText.color;
            color.a -= (Time.deltaTime * apVelocity);
            if (color.a < 0.0f)
                color.a = 0.0f;
            childText.color = color;
        }

        if (1.05f < effectTime)
            Destroy(this.gameObject);
    }

    public void InitDamage(float damage, Color color)
    {
        if (childText == null)
            childText = this.GetComponentInChildren<Text>();

        if (damage <= 0.0f)
        {
            int dmgNum = (int)Mathf.Abs(damage);
            childText.text = "- " + dmgNum;
        }
        else
        {
            childText.text = "+ " + (int)damage;
        }

        color.a = 1.0f;
        childText.color = color;
    }
}
