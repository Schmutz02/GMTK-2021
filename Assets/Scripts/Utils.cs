﻿using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public static class Utils
    {
        // DOTween helpers.

        /// <summary>
        /// Chain Tweens as continuation events.
        /// This replaces the tween's OnComplete event.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="next">the next tween to run.</param>
        /// <returns></returns>
        public static Tween Then(this Tween t, Tween next)
        {
            // Don't play the tween immediately
            next.Pause();
            // Play it when the current one is done!
            t.OnComplete(() => { next.Play(); });
            return next;
        }

        /// <summary>
        /// Chain an Action as a continuation event.
        /// This replaces the tween's OnComplete event,
        /// and is essentially an alias of OnComplete.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="next">the next tween to run.</param>
        /// <returns></returns>
        public static Tween Then(this Tween t, Action then)
        {
            // Create an event that we will complete instantly.
            var next = DOWait(1f);
            // Pause it for now.
            next.Pause();
            // Play it when the current one is done!
            t.OnComplete(() =>
            {
                // Run the callback.
                then?.Invoke();
                // Trigger the next event.
                next.Kill(complete: true);
            });

            return next;
        }

        /// <summary>
        /// Uses DOTween to delay a certain amount of time.
        /// This is most useful when combined with .Then(...) or .OnComplete(...)
        /// </summary>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public static Tween DOWait(float seconds)
        {
            // Empty tween that just delays some time.
            float start = 0f;
            return DOTween.To(() => start, v => start = v, 1f, duration: seconds);
        }
    }
}