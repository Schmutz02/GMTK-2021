using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class Playback : MonoBehaviour
    {
        public AudioClip DrumSound;

        public LineRenderer Path;

        [HideInInspector]
        public AudioSource Music;

        private bool _playingBack;

        public void Awake()
        {
            Music = gameObject.AddComponent<AudioSource>();
            _sfxSources = new HashSet<AudioSource>();
            _hitmarks = new List<HitMarkObject>();
        }

        private List<HitMarkObject> _hitmarks;
        public void StartPlayback(AudioClip audio, List<HitMark> times)
        {
            // temp, clear out previous hitmark objects
            _hitmarks.Clear();

            // First, let's create all of the necessary HitMarkObjects
            foreach (var hitmark in times)
            {
                var obj = new GameObject();
                var mark = obj.AddComponent<HitMarkObject>();
                mark.Hitmark = hitmark;
                mark.Parent = this;
                _hitmarks.Add(mark);
            }

            Music.clip = audio;
            Music.PlayDelayed(0.5f);
            Utils.DOWait(0.51f).Then(() =>
            {
                _playingBack = true;
            });
        }

        public void OnHitMarkArrived(HitMarkObject obj)
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

            // remove the object from the tracked list
            _hitmarks.Remove(obj);
        }

        private HashSet<AudioSource> _sfxSources;
        public void Update()
        {
            //if (!_music.isPlaying)
            //{
            //    // we done, idk
            //}
            //else
            //{
            //    //var c = Background.color;
            //    //c.a -= Time.deltaTime * 5f;
            //    //Background.color = c;
            //    bool playedDrum = false;

            //    foreach (var hitmarkObj in _hitmarks)
            //    {
            //        var hitmark = hitmarkObj.Data;
            //    }

            //    //// temp way of just seeing how accurate recording is. this should be a queue or something
            //    //foreach (var hitmark in _playbackTimes.ToArray())
            //    //{
            //    //    if (_music.time >= hitmark.Time)
            //    //    {
            //    //        _playbackTimes.Remove(hitmark);
            //    //        if (hitmark.Type == HitType.Red)
            //    //        {
            //    //            Background.color = Color.red;
            //    //        }
            //    //        else if (hitmark.Type == HitType.Blue)
            //    //        {
            //    //            Background.color = Color.blue;
            //    //        }
            //    //        if (!playedDrum)
            //    //        {
            //    //            var sfx = gameObject.AddComponent<AudioSource>();
            //    //            sfx.playOnAwake = false;
            //    //            sfx.volume = 0.75f;
            //    //            sfx.clip = DrumSound;
            //    //            sfx.Play();
            //    //            _sfxSources.Add(sfx);

            //    //            playedDrum = true;
            //    //        }
            //    //    }
            //    //    else break;
            //    //}
            //}

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
