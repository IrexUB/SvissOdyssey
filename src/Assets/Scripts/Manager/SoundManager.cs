using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoSingleton<SoundManager>
{
	//Dictionnaire du temps d'attente pour chaque son
	[SerializeField]public static Dictionary<GameUtils.SoundList, SourceAudio> soundDictionnary;
    public void FillDictionnary()
    {
        soundDictionnary = new Dictionary<GameUtils.SoundList, SourceAudio> { };
        for (int i = 0; i < GameManager.instance.SoundList.Count - 1; i++)
        {
            soundDictionnary.Add(GameManager.instance.SoundList[i], GameManager.instance.SourceAudioListe[i]);
        }
    }
    public static void Create(GameUtils.SoundList _sound, Vector3 _pos, Quaternion _rot)
	{
		GameObject soundToPlay = Instantiate(soundDictionnary[_sound].audioClip, _pos, _rot);
        if (_sound != GameUtils.SoundList.MUSIC_ECRAN_TITRE && _sound != GameUtils.SoundList.MUSIC_ECRAN_GAME_OVER && _sound != GameUtils.SoundList.MUSIC_DONJON_2 && _sound != GameUtils.SoundList.MUSIC_DONJON_1 && _sound != GameUtils.SoundList.MUSIC_CINEMATIQUE && _sound != GameUtils.SoundList.MUSIC_BOSS_1 && _sound != GameUtils.SoundList.MUSIC_BOSS_2)
        {
            Destroy(soundToPlay, 5f);
        }
	}
}