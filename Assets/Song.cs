using TMPro;
using UnityEngine;

public class Song : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;
    public AudioClip Audio { get; private set; }

    public void Init(AudioClip audioClip)
    {
        Audio = audioClip;

        _nameText.text = audioClip.name;
    }
}