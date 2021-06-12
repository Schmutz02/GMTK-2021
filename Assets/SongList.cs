using UnityEngine;

public class SongList : MonoBehaviour
{
    [SerializeField]
    private Transform _songListTransform;

    [SerializeField]
    private Song _songPrefab;

    private void Awake()
    {
        var songs = Resources.LoadAll<AudioClip>("Audio/Songs");
        foreach (var clip in songs)
        {
            var song = Instantiate(_songPrefab, _songListTransform);
            song.Init(clip);
            song.gameObject.SetActive(true);
        }
    }
}
