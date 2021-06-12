using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Recorder : MonoBehaviour
    {
        public Button StopButton;
        public Button SaveButton;
        public Button DeleteButton;
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
            
            SaveButton.onClick.AddListener(() =>
            {
                _saveRecording();
                StopButton.gameObject.SetActive(true);
                SaveButton.gameObject.SetActive(false);
                DeleteButton.gameObject.SetActive(false);
                Playback.StopPlayback();
            });
            DeleteButton.onClick.AddListener(() =>
            {
                _recTimes.Clear();
                StopButton.gameObject.SetActive(true);
                SaveButton.gameObject.SetActive(false);
                DeleteButton.gameObject.SetActive(false);
                Playback.StopPlayback();
            });

            StopButton.onClick.AddListener(() =>
            {
                _music.Stop();
                _recording = false;

                // steal times for test playback
                var times = _recTimes;
                
                StopButton.gameObject.SetActive(false);
                SaveButton.gameObject.SetActive(true);
                DeleteButton.gameObject.SetActive(true);
                
                Playback.StartPlayback(Audio, times);
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

        public void StartRecording(Song song)
        {
            Audio = song.Audio;
            StopButton.gameObject.SetActive(true);
            _startRecording();
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