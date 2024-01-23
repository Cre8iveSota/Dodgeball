using UnityEngine;
using DG.Tweening;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSourceSE; // SE source
    public AudioClip[] audioClipsSE; // item


    public AudioSource audioSourceBGM; // BGM source
    public AudioClip[] audioClipsBGM; // item of BGM(0:Menu 1:Game)
    public static SoundManager instance;
    private readonly int totalPlayingTweens = DOTween.TotalPlayingTweens();
    int seCount = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //Ensure that sound is not interrupted when transitioning between scenes
            // DontDestroyOnLoad(this.gameObject.transform.GetChild(0));
            DontDestroyOnLoad(this.gameObject);

            // DOTweenの初期化
            DOTween.Init();

            // トゥイーンの容量を手動で設定
            DOTween.SetTweensCapacity(2000, 100);
        }
        else
        {
            // If this SoundManager is prefabricated elsewhere, remove it to prevent the same sound from being played back and creating a multiplicity of sounds.
            Destroy(this.gameObject);
        }
    }


    public void StopBGM()
    {
        audioSourceBGM.Stop();
    }
    public void PlayBGM(int index)
    {
        float fadeOutDuration = 1f;
        audioSourceBGM.DOFade(0f, fadeOutDuration).OnComplete(() =>
        {
            // BGM 停止
            audioSourceBGM.Stop();

            // 新しい BGM を設定

            audioSourceBGM.clip = audioClipsBGM[index];

            // BGM フェードイン
            float fadeInDuration = 1f;
            audioSourceBGM.volume = 0f;
            audioSourceBGM.Play();
            audioSourceBGM.DOFade(0.5f, fadeInDuration);
        });
    }

    public void PlaySE(int index)
    {
        audioSourceSE.PlayOneShot(audioClipsSE[index]); // SEを一度だけ鳴らす   
    }

    public void PlayOnlyThisSE(int index)
    {
        audioSourceSE.Stop(); // スペースキーを連打すると、alertが多重起動してうるさいため
        audioSourceSE.PlayOneShot(audioClipsSE[index]); // SEを一度だけ鳴らす   
    }


    public void StopAllSE()
    {
        if (audioSourceSE.isPlaying)
        {
            DOTween.KillAll(true);
            audioSourceSE.Stop();
        }
    }

    public void PlayClearBGM()
    {
        Debug.Log("totalPlayingTweens clear" + totalPlayingTweens);

        audioSourceBGM.Stop();
        StopAllSE();
        float fadeOutDuration = 1f;
        audioSourceBGM.DOFade(0f, fadeOutDuration).OnComplete(() =>
        {
            // BGM 停止

            audioSourceBGM.clip = audioClipsBGM[2];
            float fadeInDuration = 1f;
            audioSourceBGM.volume = 0f;
            audioSourceBGM.Play();
            audioSourceBGM.DOFade(1f, fadeInDuration);
        });
    }

}