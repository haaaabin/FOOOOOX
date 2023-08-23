using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBullCtrl : MonoBehaviour
{
    [HideInInspector] public GameObject player;
    [HideInInspector] public float m_DownSpeed = 10f;

    [HideInInspector] public float spawn = 2.0f;
    [HideInInspector] public float delta = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        this.player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0.0f, -m_DownSpeed * Time.deltaTime, 0.0f);

        if (transform.position.y < -6.5f)
            Destroy(gameObject);
    }

}
