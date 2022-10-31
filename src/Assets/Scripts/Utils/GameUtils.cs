using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using UnityEngine;

public class GameUtils
{
    public enum GameStates
    {
        WAIT_PLAYER,
        TIMER,
        PLAY
    }

	public enum Effects
    {
		NONE,
		POISON,
		REGENERATION,
		INSTANT_HEAL,
		STUN
    }
	public enum SoundList
	{
		MUSIC_ECRAN_TITRE,
		MUSIC_CINEMATIQUE,
		MUSIC_DONJON_1,
		MUSIC_DONJON_2,
		MUSIC_ECRAN_GAME_OVER,
		MUSIC_BOSS_1,
		MUSIC_BOSS_2,
		COIN,
		COUP_EPEE,
		COUP_RECU,
		COUTEAU,
		EXPLOSION,
		FIREBALL,
		LANCER_COUTEAU,
		MORT,
		SQUELETTE,
		SWING_EPEE,
		VERRE_BRISE,
		ZOMBIE
	}

	public static string GetLocalIPAddress()
	{
		var host = Dns.GetHostEntry(Dns.GetHostName());
		foreach (var ip in host.AddressList)
		{
			if (ip.AddressFamily == AddressFamily.InterNetwork)
			{
				return ip.ToString();
			}
		}

		return "IPv4 not found";
	}

	// Fonction qui change de statut d'un GameObject (Activation / Désactivation)
	public static void DisableGameObject(GameObject obj, bool state = false)
    {
		obj.SetActive(state);
    }

	// Fonction qui génère un nombre sur le périmètre d'un cercle
	public static Vector2 GetRandomPosOnCirclePerimeter(Vector2 pos, float radius)
    {
		int angle = Random.Range(0, 359);
		Vector2 randPos = new Vector2(pos.x + radius * Mathf.Cos(angle), pos.y + radius * Mathf.Sin(angle));

		return randPos;
	}

}
