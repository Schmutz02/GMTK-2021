using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class Playback : MonoBehaviour
    {
        public AudioClip DrumSound;

        public LineRenderer Path;

        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI ReactionText;

        public int Score;

        [HideInInspector]
        public AudioSource Music;

        private bool _playingBack;

        public void Awake()
        {
            Music = gameObject.AddComponent<AudioSource>();
            _sfxSources = new HashSet<AudioSource>();
            _hitmarks = new List<HitMarkObject>();
            _next = new Dictionary<HitType, HitMarkObject>();
            _next[HitType.Blue] = null;
            _next[HitType.Red] = null;
        }

        private Dictionary<HitType, HitMarkObject> _next;
        private List<HitMarkObject> _hitmarks;
        public void StartPlayback(AudioClip audio, List<HitMark> times)
        {
            // temp, clear out previous hitmark objects
            _hitmarks.Clear();

            // reset score
            Score = 0;
            UpdateScore();

            // reset reactions
            ReactionText.text = "";

            // First, let's create all of the necessary HitMarkObjects
            foreach (var hitmark in times)
            {
                var obj = new GameObject();
                var mark = obj.AddComponent<HitMarkObject>();
                mark.Hitmark = hitmark;
                mark.Parent = this;
                _hitmarks.Add(mark);

                if (_next[HitType.Blue] == null && hitmark.Type == HitType.Blue)
                {
                    _next[HitType.Blue] = mark;
                    _next[HitType.Blue].SetAsNext();
                }
                if (_next[HitType.Red] == null && hitmark.Type == HitType.Red)
                {
                    _next[HitType.Red] = mark;
                    _next[HitType.Red].SetAsNext();
                }
            }

            Music.clip = audio;
            Music.PlayDelayed(0.5f);
            Utils.DOWait(0.51f).Then(() =>
            {
                _playingBack = true;
            });
        }

        public void UpdateScore()
        {
            // displays score as 0000000
            ScoreText.text = $"{Score}".PadLeft(7, '0');
        }

        public void OnHitMarkTapped(HitMarkObject obj, float error)
        {
            if (Mathf.Abs(error) < 0.15f)
            {
                // for now, let's just uhhh play the drum sound
                var sfx = gameObject.AddComponent<AudioSource>();
                sfx.playOnAwake = false;
                sfx.volume = 0.75f;
                sfx.clip = DrumSound;
                sfx.Play();

                // slightly modify pitch, but make sure that we use loose time as a seed
                // that way beats that are grouped together don't sound like alien lasers
                UnityEngine.Random.InitState(Mathf.RoundToInt(Time.time));
                sfx.pitch += (UnityEngine.Random.value) / 20f;

                // Modify score based on error
                // were we really close? todo: bpm math
                float err = Mathf.Abs(error);
                int toAdd = 0;
                string reaction = "";
                if (err < 0.04f)
                {
                    // osu scoring kEKW
                    toAdd = 300;
                    reaction = "Perfect!";
                }
                else if (err < 0.07f)
                {
                    // osu scoring kEKW
                    toAdd = 100;
                    reaction = "Cool!";
                }
                else if (err < 0.12f)
                {
                    // osu scoring kEKW
                    toAdd = 50;
                    reaction = "It's ok!";
                }

                Debug.Log($"Error: {err}");

                Score += toAdd;
                UpdateScore();

                // todo: juice this
                ReactionText.text = reaction;

                _sfxSources.Add(sfx);
            }
            else
            {

            }

            // remove the object from the tracked list
            _hitmarks.Remove(obj);

            // let's mark the next one! search for next matching color..
            foreach (var hit in _hitmarks)
            {
                if (hit.Hitmark.Type == obj.Hitmark.Type)
                {
                    _next[obj.Hitmark.Type] = hit;
                    _next[obj.Hitmark.Type].SetAsNext();
                    break;
                }
            }
        }

        private HashSet<AudioSource> _sfxSources;
        public void Update()
        {
            foreach (var drumSound in _sfxSources.ToArray())
            {
                if (!drumSound.isPlaying)
                {
                    _sfxSources.Remove(drumSound);
                    Destroy(drumSound);
                }
            }
        }
    }
}
