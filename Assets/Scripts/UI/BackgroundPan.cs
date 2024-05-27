using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundPan : MonoBehaviour
{
    public float speed = 1;
    RectTransform _transform;
    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {

        _transform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        _transform.anchoredPosition += new Vector2(speed, 0) * Time.deltaTime;
        if(_transform.anchoredPosition.x > 835)
        {
            _transform.anchoredPosition = new Vector2(835, 0);
            speed *= -1;
        }
        if (_transform.anchoredPosition.x < -835)
        {
            _transform.anchoredPosition = new Vector2(-835, 0);
            speed *= -1;
        }

    }
}
