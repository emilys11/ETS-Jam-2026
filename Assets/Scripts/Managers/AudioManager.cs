using System;
using UnityEngine;
using UnityEngine.Audio;

    public class AudioHandler : MonoBehaviour
    {
        //Singleton
        private static AudioHandler _instance;
        public static AudioHandler Instance { get { return _instance; } }

        private AudioSource currentSource;
        [Header("Audio Mixers")]
        [SerializeField] private AudioMixer audioMixer;

        [Header("Musics")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip gameMusic;

        [Header("Main Menu Audio")]
        [SerializeField] public AudioClip selectEffect;

        [Header("Game Audio")]
        [SerializeField] public AudioClip spawnEffect;
        [SerializeField] public AudioClip crushedEffect;
        [SerializeField] public AudioClip meteorLanding;
        [SerializeField] public AudioClip volcanoEffect;
        [SerializeField] public AudioClip charredEffect;
        [SerializeField] public AudioClip flooddEffect;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
        {
            DontDestroyOnLoad(gameObject.transform);
            SetMusicSource(musicSource, gameMusic);
            musicSource.Play();
            //musicSource.Pause();
        }

        private void ChangeAudioSource(AudioSource source, bool forceRestart = false)
        {
            if (currentSource == source && !forceRestart) return;

            if (currentSource != null)
                currentSource.Stop();

            currentSource = source;

            if (currentSource.clip != null)
            {
                currentSource.time = 0f;
                currentSource.Play();
            }
        }

        private void SetMusicSource(AudioSource music, AudioClip clip)
        {
            if (music == musicSource)
            {
                musicSource.clip = clip;
                return;
            }
        }

        private float currentPitch = 1.0f;
        public float pitchSmoothSpeed = 5.0f;



        public void PlayEffect(AudioClip effect, string groupName)
        {
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = Camera.main.transform.position;

            AudioSource source = tempGO.AddComponent<AudioSource>();
            source.clip = effect;

            var groups = audioMixer.FindMatchingGroups(groupName);
            if (groups.Length > 0)
            {
                source.outputAudioMixerGroup = groups[0];
            }
            else
            {
                source.outputAudioMixerGroup = audioMixer.FindMatchingGroups("SFX")[0];
            }

            source.Play();
            Destroy(tempGO, effect.length);
        }
       
    }
