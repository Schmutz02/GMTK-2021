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
            else
            {
                float dist = Parent.Music.time - Hitmark.Time;
                if (dist <= 0)
                {
                    Vector3 start = Parent.Path.GetPosition(0);
                    Vector3 end = _resolveTargetFromType(Hitmark.Type);
                    Vector3 diff = end - start;
                    Vector3 diffFromTarget = start - end;
                    Vector3 offsetFromTarget = diffFromTarget * dist;
                    transform.position = end - offsetFromTarget;
                }
                else
                {
                    if (_fadeOut == null)
                    {
                        Parent.OnHitMarkArrived(this);
                        var seq = DOTween.Sequence();
                        seq.Append(_renderer.DOColor(Color.clear, 0.1f));
                        seq.Append(transform.DOScale(20f, 0.1f));
                        _fadeOut = seq.Play();
                    }
                }
            }
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
    }
}
