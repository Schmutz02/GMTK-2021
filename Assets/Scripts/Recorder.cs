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
        public AudioClip DrumSound;

        public RawImage Background;

        private AudioSource _source;

        // Start is called before the first frame update
        public void Start()
        {
            _source = gameObject.AddComponent<AudioSource>();
            _source.playOnAwake = false;

            _recTimes = new List<HitMark>();

            TempStartButton.onClick.AddListener(() =>
            {
                if (!_recording)
                    _startRecording();
                else
                {
                    _source.Stop();
                    _recording = false;

                    // this does nothing right now
                    _saveRecording();

                    // steal times for test playback
                    var times = _recTimes;
                    _startPlayback(Audio, times);

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
            else if (_playingBack)
            {
                // playback loop (this will move to another script)
                _tickPlayback();
            }
        }

        private enum HitType
        {
            Red,
            Blue
        }
        private struct HitMark
        {
            public HitType Type;
            public float Time;
        }


        private List<HitMark> _recTimes;
        private bool _recording;
        private bool _playingBack;
        private void _startRecording()
        {
            // we should save the output or smth...
            _source.clip = Audio;
            _source.PlayDelayed(0.5f);
            Utils.DOWait(0.51f).Then(() => _recording = true);
        }
        private void _tickRecording()
        {
            if (!_source.isPlaying)
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
                        Time = _source.time,
                        Type = HitType.Red
                    });
                    // play sfx
                    _source.PlayOneShot(DrumSound);
                }
                if (bluePressed)
                {
                    // store timestamp
                    _recTimes.Add(new HitMark
                    {
                        Time = _source.time,
                        Type = HitType.Blue
                    });
                    // play sfx
                    _source.PlayOneShot(DrumSound);
                }
            }
        }
        private void _saveRecording()
        {
            foreach (var hitmark in _recTimes)
            {
                Debug.Log($"{hitmark.Type} at {hitmark.Time}");
            }
        }

        // temp, this needs to move to a playback script
        private List<HitMark> _playbackTimes;
        private float _playbackStartTime;
        private void _startPlayback(AudioClip audio, List<HitMark> times)
        {
            _source.clip = Audio;
            _source.PlayDelayed(0.5f);
            _playbackTimes = times;
            Utils.DOWait(0.51f).Then(() =>
            {
                _playingBack = true;
                _playbackStartTime = Time.time;
            });
        }

        private void _tickPlayback()
        {
            if (!_source.isPlaying)
            {
                // we done, idk
            }
            else
            {
                var c = Background.color;
                c.a -= Time.deltaTime * 5f;
                Background.color = c;
                bool playedDrum = false;
                var unitySourceTime = Time.time - _playbackStartTime;
                var timeDiff = unitySourceTime - _source.time;
                // temp way of just seeing how accurate recording is. this should be a queue or something
                foreach (var hitmark in _playbackTimes.ToArray())
                {
                    if (_source.time >= hitmark.Time + timeDiff)
                    {
                        _playbackTimes.Remove(hitmark);
                        if (hitmark.Type == HitType.Red)
                        {
                            Background.color = Color.red;
                        }
                        else if (hitmark.Type == HitType.Blue)
                        {
                            Background.color = Color.blue;
                        }
                        if (!playedDrum)
                            _source.PlayOneShot(DrumSound);
                    }
                    else break;
                }
            }
        }
    }
}