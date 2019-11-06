using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public AudioClip[] Songs;

    public AudioClip[] GrassMaterial, DirtMaterial, RockMaterial, WoodMaterial, SandMaterial, GravelMaterial, ClothMaterial, SnowMaterial;
    public Dictionary<string, AudioClip[]> MaterialSounds;

    private AudioSource audioSource;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        MaterialSounds = new Dictionary<string, AudioClip[]>();
        MaterialSounds.Add("grass", GrassMaterial);
        MaterialSounds.Add("dirt", DirtMaterial);
        MaterialSounds.Add("rock", RockMaterial);
        MaterialSounds.Add("wood", WoodMaterial);
        MaterialSounds.Add("sand", SandMaterial);
        MaterialSounds.Add("gravel", GravelMaterial);
        MaterialSounds.Add("cloth", ClothMaterial);
        MaterialSounds.Add("snow", SnowMaterial);
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() {
        /*
        if (!audioSource.isPlaying) {
            PlaySong();
        }
        */
    }

    private void PlaySong() {
        audioSource.clip = Songs[Random.Range(0, Songs.Length)];
        audioSource.Play();
    }

    public void PlayMaterialSound(string material, Vector3 position) {
        if (material == "none")
            return;
        AudioClip[] _sounds = MaterialSounds[material];
        PlaySound3D(_sounds[Random.Range(0, _sounds.Length)], 0.9f, position, 1.0f, Random.Range(0.8f, 1.1f));
    }

    public static void PlaySound(AudioClip clip, float volume, float pitch) {
        GameObject _sfx = new GameObject("SFX_" + clip.name);
        AudioSource _as = _sfx.AddComponent<AudioSource>();
        _as.volume = volume;
        _as.pitch = pitch;
        _as.clip = clip;
        _as.Play();
        Destroy(_sfx, clip.length);
    }

    public static void PlaySound3D(AudioClip clip, float spacialblend, Vector3 position, float volume, float pitch) {
        GameObject _sfx = new GameObject("SFX_" + clip.name);
        _sfx.transform.position = position;
        AudioSource _as = _sfx.AddComponent<AudioSource>();
        _as.spatialBlend = spacialblend;
        _as.volume = volume;
        _as.pitch = pitch;
        _as.clip = clip;
        _as.Play();
        Destroy(_sfx, clip.length);
    }
}
