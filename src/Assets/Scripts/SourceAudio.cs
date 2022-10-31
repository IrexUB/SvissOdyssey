using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/AudioSource")]
public class SourceAudio : ScriptableObject
{
	public GameUtils.SoundList sound;
	public GameObject audioClip;
}
