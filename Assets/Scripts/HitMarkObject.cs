using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class HitMarkObject : MonoBehaviour
    {
        public HitMark Hitmark;

        [HideInInspector]
        public Playback Parent;

        public const float LIFETIME = 2f;

        private SpriteRenderer _renderer;
        private Tween _fadeOut;

        private bool _next;

        public void Start()
        {
            _renderer = gameObject.AddComponent<SpriteRenderer>();
            _renderer.sprite = ResourceHolder.GetHitmarkSprite(Hitmark.Type);
            _renderer.sortingOrder = 5;

            transform.position = Parent.Path.GetPosition(0);
            transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }

        public void Update()
        {
            if (Mathf.Abs(Parent.Music.time - Hitmark.Time) > LIFETIME)
            {
                // this object should be hidden
            }
            else if (_leaving)
            {
                // we're leaving. chill
            }
            else
            {
                float dist = Parent.Music.time - Hitmark.Time;
                if (dist <= -0.15f)
                {
                    // just move along normally
                    _updatePosition(dist);
                }
                else
                {
                    // still move along normally
                    _updatePosition(dist);
                    // but also check for hits
                    if (_next)
                    {
                        if (dist <= 0.15f)
                        {
                            // todo: keybinds
                            if (Input.GetKeyDown(KeyCode.Z) && Hitmark.Type == HitType.Red)
                            {
                                Parent.OnHitMarkTapped(this, dist);
                                _destroy();
                            }
                            if (Input.GetKeyDown(KeyCode.X) && Hitmark.Type == HitType.Blue)
                            {
                                Parent.OnHitMarkTapped(this, dist);
                                _destroy();
                            }
                        }
                        else
                        {
                            // too late. auto-miss
                            if (!_leaving)
                            {
                                Parent.OnHitMarkTapped(this, 10000f);
                                _destroy();
                            }
                        }
                    }
                }
            }
        }

        public void SetAsNext()
        {
            _next = true;
        }

        private void _updatePosition(float dist)
        {
            Vector3 start = Parent.Path.GetPosition(0);
            Vector3 end = _resolveTargetFromType(Hitmark.Type);
            Vector3 diff = end - start;
            Vector3 diffFromTarget = start - end;
            Vector3 offsetFromTarget = diffFromTarget * dist;
            transform.position = end - offsetFromTarget;
        }

        private Vector3 _resolveTargetFromType(HitType type)
        {
            if (type == HitType.Red)
                return Parent.Path.transform.GetChild(0).position;
            if (type == HitType.Blue)
                return Parent.Path.transform.GetChild(1).position;

            // wtf
            return Vector3.zero;
        }

        private bool _leaving;
        private void _destroy(bool good = true)
        {
            float targetScale = good ? 1f : 0f;

            _next = false;
            _leaving = true;
            var seq = DOTween.Sequence();
            seq.Join(_renderer.DOColor(Color.clear, 0.1f));
            seq.Join(transform.DOScale(targetScale, 0.1f));
            _fadeOut = seq.Play().Then(() => Destroy(this));
        }
    }
}
