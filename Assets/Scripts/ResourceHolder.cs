using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class ResourceHolder : MonoBehaviour
    {
        private static ResourceHolder _instance;
        public void Awake()
        {
            _instance = this;
        }

        public Sprite BlueHitmark;
        public Sprite RedHitmark;

        public static Sprite GetHitmarkSprite(HitType type)
        {
            if (type == HitType.Blue)
                return _instance.BlueHitmark;
            if (type == HitType.Red)
                return _instance.RedHitmark;

            return null;
        }
    }
}
