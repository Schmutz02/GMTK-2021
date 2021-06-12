using DG.Tweening;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Utils
    {
        public static void SaveNoteRecordingToFile(List<HitMark> notes, string songName)
        {
            using (var wtr = new StreamWriter($"Assets/Resources/Notes/{songName}.txt"))
            {
                wtr.WriteLine(songName);
                foreach (var note in notes)
                {
                    var noteData = $"{note.Time},{note.Type}";
                    wtr.WriteLine(noteData);
                }
            }
        }

        public static List<HitMark> LoadNoteRecordingFromFile(string songName)
        {
            if (!File.Exists($"Assets/Resources/Notes/{songName}.txt"))
                return null;
            
            var notes = new List<HitMark>();
            using (var rdr = new StreamReader($"Assets/Resources/Notes/{songName}.txt"))
            {
                var savedSongName = rdr.ReadLine();
                if (savedSongName != songName)
                {
                    Debug.Log("Different song found");
                    return null;
                }

                string line;
                while ((line = rdr.ReadLine()) != null)
                {
                    var noteData = line.Split(',');
                    var note = new HitMark()
                    {
                        Time = float.Parse(noteData[0]),
                        Type = (HitType) Enum.Parse(typeof(HitType), noteData[1])
                    };
                    
                    notes.Add(note);
                }
            }

            return notes;
        }
        
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
