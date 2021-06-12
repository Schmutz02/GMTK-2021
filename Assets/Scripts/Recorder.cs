using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Recorder : MonoBehaviour
    {
        public Button TempStartButton;
        public AudioClip Audio;
        public AudioClip DrumSound;

        public Playback Playback;

        public RawImage Background;

        private AudioSource _music;

        // Start is called before the first frame update
        public void Start()
        {
            _music = gameObject.AddComponent<AudioSource>();
            _music.playOnAwake = false;

            _recTimes = new List<HitMark>();

            TempStartButton.onClick.AddListener(() =>
            {
                if (!_recording)
                    _startRecording();
                else
                {
                    _music.Stop();
                    _recording = false;
                    
                    _saveRecording();

                    // steal times for test playback
                    var times = _recTimes;

                    var notes = Utils.LoadNoteRecordingFromFile(Audio.name);
                    Playback.StartPlayback(Audio, notes);

                    // clear
                    _recTimes = new List<HitMark>();
                    _recTimes.Clear();
                }
            });
        }

        // Update is called once per frame
        public void Update()
        {
            // for now, let's assume that we're just using osu z/x keybinds and mouse buttons
            if (_recording)
            {
                // record loop
                _tickRecording();
            }
            //else if (_playingBack)
            //{
            //    // playback loop (this will move to another script)
            //    _tickPlayback();
            //}
        }

        private List<HitMark> _recTimes;
        private bool _recording;
        private void _startRecording()
        {
            // we should save the output or smth...
            _music.clip = Audio;
            _music.PlayDelayed(0.5f);
            Utils.DOWait(0.51f).Then(() => _recording = true);
        }
        private void _tickRecording()
        {
            if (!_music.isPlaying)
            {
                // made it all the way through!
                // for now let's just output the magic
                _saveRecording();
                _recTimes.Clear();
            }
            else
            {
                bool redPressed = Input.GetKeyDown(KeyCode.Z);// || Input.GetMouseButtonDown(0);
                bool bluePressed = Input.GetKeyDown(KeyCode.X);// || Input.GetMouseButtonDown(1);
                if (redPressed)
                {
                    // store timestamp
                    _recTimes.Add(new HitMark
                    {
                        Time = _music.time,
                        Type = HitType.Red
                    });
                    // play sfx
                    _music.PlayOneShot(DrumSound);
                }
                if (bluePressed)
                {
                    // store timestamp
                    _recTimes.Add(new HitMark
                    {
                        Time = _music.time,
                        Type = HitType.Blue
                    });
                    // play sfx
                    _music.PlayOneShot(DrumSound);
                }
            }
        }
        private void _saveRecording()
        {
            Utils.SaveNoteRecordingToFile(_recTimes, Audio.name);
        }
    }
}