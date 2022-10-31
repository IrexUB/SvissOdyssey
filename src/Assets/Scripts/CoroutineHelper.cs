using System.Collections;
using UnityEngine;

// Cette classe permet le lancement de couroutine depuis des ScriptableObjects
public class CoroutineHelper : MonoBehaviour
{
    public static CoroutineHelper instance;

    private void Start()
    {
        CoroutineHelper.instance = this;
    }
}