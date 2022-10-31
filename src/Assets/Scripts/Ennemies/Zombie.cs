using UnityEngine;

public class Zombie : PEnemyController
{
	[SerializeField] private float distanceToAttack = 1f;
	[SerializeField] private const float MAX_ATTACK_TIME = 2f;
	[SerializeField] private float attackTime = MAX_ATTACK_TIME;

	private bool canAttack = false;

	protected override void Update()
	{
		base.Update();

		// Si on a une target
		if (playerTarget)
		{
			float distance = Vector2.Distance(transform.position, playerTarget.position);
			if (distance <= distanceToAttack)
			{
				canAttack = true;
				if (attackTime <= 0)
				{
					// Attaque du joueur
					Attack();
				}
			} else
            {
				canAttack = false;
            }

			attackTime -= Time.deltaTime;
		}
	}

    protected override void FixedUpdate()
    {
		// Si ce n'est pas le serveur
		if (!isServer) return;

		if (canAttack)
		{
			rb.velocity = Vector2.zero;
			return;
		}
	
        base.FixedUpdate();
    }

    private void Attack()
	{
		// Récupération du script de vie de la target
		if(playerTarget.TryGetComponent(out Health pHealth)) {
			pHealth.ServerTakeDamage(stats.m_physicalAttack);

			attackTime = MAX_ATTACK_TIME;
		}
	}
}
