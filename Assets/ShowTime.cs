using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class ShowTime : MonoBehaviour
{
    private TMP_Text _timeText;

    // Start is called before the first frame update
    void Start()
    {
        if (GetComponent<TMP_Text>()  != null)
        {
            _timeText = GetComponent<TMP_Text>();
        }

    }

    // Update is called once per frame
    void Update()
    {
        _timeText.text = DateTime.Now.ToString("HH:mm");
    }
}
