using System;
using UnityEngine;
using UnityEngine.UI;

public class Song : MonoBehaviour
{
    [SerializeField]
    private Text _nameText;

    [SerializeField]
    private Button _button;
    
    public AudioClip Audio { get; private set; }

    private void Awake()
    {
        //register button things
    }

    public void Init(AudioClip audioClip)
    {
        Audio = audioClip;

        _nameText.text = audioClip.name;
    }
}