using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Recorder : MonoBehaviour
    {
        public Button TempStartButton;
        public AudioClip Audio;

        private AudioSource _source;

        // Start is called before the first frame update
        public void Start()
        {
            _source = gameObject.AddComponent<AudioSource>();

            _times = new Dictionary<HitType, List<float>>();
            _times[HitType.Red] = new List<float>();
            _times[HitType.Blue] = new List<float>();

            TempStartButton.onClick.AddListener(() => { _startRecording(); });
        }

        // Update is called once per frame
        public void Update()
        {
            // for now, let's assume that we're just using osu z/x keybinds and mouse buttons
            if (_recording)
            {
                if (!_source.isPlaying)
                {
                    // made it all the way through!
                    // todo: the speed up magic and all that

                }

                bool redPressed = Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(0);
                bool bluePressed = Input.GetKeyDown(KeyCode.X) || Input.GetMouseButtonDown(1);
                if (redPressed)
                {
                    // store timestamp
                    _times[HitType.Red].Add(_source.time);
                }
                if (bluePressed)
                {
                    // store timestamp
                    _times[HitType.Blue].Add(_source.time);
                }
            }
        }

        private enum HitType
        {
            Red,
            Blue
        }
        private Dictionary<HitType, List<float>> _times;
        private bool _recording;

        private void _startRecording()
        {
            // we should save the output or smth...
            _source.clip = Audio;
            _source.PlayDelayed(0.5f);
            Utils.DOWait(0.51f).Then(() => _recording = true);
        }
        private void _saveRecording()
        {

        }
    }
}