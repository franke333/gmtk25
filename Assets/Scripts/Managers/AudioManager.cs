using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : SingletonClass<AudioManager>
{

    [SerializeField]
    private AudioClip PlaceDownClip, NotAllowedClip, LoseClip, ShootClip, UpgradeClip;
    [SerializeField]
    private List<AudioClip> musicList;
    private int currentMusicIndex = -1;

    private AudioSource placeDownSource, notAllowedSource, loseSource, upgradeSource, musicSource;
    private List<AudioSource> shootSource = new List<AudioSource>();
    private int shootIndex = 0;

    public Slider sliderSFXm ,musicSlider;

    public float VolumeSFX
    {
        get { return sliderSFXm.value; }
        set { sliderSFXm.value = value; SetSFXVolume(value); }
    }

    public float VolumeMusic
    {
        get { return musicSlider.value; }
        set { musicSlider.value = value; musicSource.volume = value; }
    }


    public void PlayPlaceDown()
    {
        placeDownSource.clip = PlaceDownClip;
        placeDownSource.Play();
    }

    public void PlayNotAllowed()
    {
        notAllowedSource.clip = NotAllowedClip;
        notAllowedSource.Play();
    }

    public void PlayLose()
    {
        loseSource.clip = LoseClip;
        loseSource.Play();
    }
    public void PlayUpgrade()
    {
        upgradeSource.clip = UpgradeClip;
        upgradeSource.Play();
    }

    public void PlayShoot()
    {
        shootSource[shootIndex].clip = ShootClip;
        shootSource[shootIndex].Play();
        shootIndex = (shootIndex + 1) % shootSource.Count;
    }

    private void Update()
    {
        if (!musicSource.isPlaying)
            PlayMusic();
    }

    public void SetSFXVolume(float value)
    {
        foreach (var source in shootSource)
        {
            source.volume = value;
        }
        placeDownSource.volume = value;
        notAllowedSource.volume = value;
        loseSource.volume = value;
        upgradeSource.volume = value;
    }


    public void PlayMusic()
    {
        //get new index
        int newIndex = Random.Range(0, musicList.Count);
        if(newIndex == currentMusicIndex)
        {
            newIndex = (newIndex + 1) % musicList.Count; // Ensure it's different from the last one
        }

        currentMusicIndex = newIndex;
        //play new music
        musicSource.clip = musicList[currentMusicIndex];
        musicSource.Play();

    }

    private void Start()
    {
        placeDownSource = gameObject.AddComponent<AudioSource>();
        notAllowedSource = gameObject.AddComponent<AudioSource>();
        loseSource = gameObject.AddComponent<AudioSource>();
        upgradeSource = gameObject.AddComponent<AudioSource>();
        musicSource = gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < 5; i++)
        {
            AudioSource newShootSource = gameObject.AddComponent<AudioSource>();
            shootSource.Add(newShootSource);
        }

        VolumeMusic = 0.66f;
        VolumeSFX = 0.5f;


        PlayMusic();
    }

}