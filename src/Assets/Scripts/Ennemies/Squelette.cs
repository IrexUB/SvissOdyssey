using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Squelette : PEnemyController
{
    [SerializeField]
    private GameObject osPrefab;
    [SerializeField]
    private float speedOs = 2f;

    private bool canLaunch = true;
    [SerializeField] private float distanceattack = 10f;
    protected override void Start()
    {
        base.Start();
    }
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (playerTarget != null)
        {
            float distance = Vector2.Distance(playerTarget.position, transform.position);
            if (distance <= distanceattack)
                StartCoroutine(LaunchBone());
        }
    }
    IEnumerator LaunchBone()
    {
        if (canLaunch)
        {
            canLaunch = false;
            //instantiation de l'os
            GameObject os = Instantiate(osPrefab, transform.position, Quaternion.identity);
            os.layer = gameObject.layer;

            // Récupération du script de l'os
            Os osScript = os.GetComponent<Os>();
            osScript.Setup(stats.m_physicalAttack);

            SpriteRenderer spriteOs = os.GetComponent<SpriteRenderer>();
            spriteOs.sortingLayerName = spriteRenderer.sortingLayerName;
            //spawn de l'os
            NetworkServer.Spawn(os);

            Vector2 directionOs = playerTarget.position - transform.position;
            os.GetComponent<Rigidbody2D>().AddForce(directionOs * speedOs, ForceMode2D.Impulse);
            yield return new WaitForSeconds(1.5f);
            //destruction de l'os
            if (os)
                NetworkServer.Destroy(os);
            canLaunch = true;
        }
    }
}
