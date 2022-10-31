using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using Mirror;

public class UIManager : MonoSingleton<UIManager>
{
	[System.Serializable]
	private struct TraderSlot
	{
		public Text priceText;
		public Text descText;
		public Image iconImage;
	}

	[System.Serializable]
	private struct StatsText
    {
		public TextMeshProUGUI health;
		public TextMeshProUGUI physicalAttack;
		public TextMeshProUGUI elementaryAttack;
		public TextMeshProUGUI defense;
		public TextMeshProUGUI elementaryDefense;
		public TextMeshProUGUI speed;
		public TextMeshProUGUI cooldownReduction;
		public TextMeshProUGUI vampirism;
	}

	[Header("WaitPlayer UI")]
	[SerializeField] private GameObject waitPlayerUI;

	[Header("Timer UI")]
	[SerializeField] private GameObject timerUI;
	[SerializeField] private Text timerText;

	[Header("IpServer UI")]
	[SerializeField] private GameObject ipServerUI;
	[SerializeField] private Text ipServerText;

	[Header("Reanimation UI")]
	[SerializeField] private GameObject reanimationUI;
	[SerializeField] private Text reanimationText;
	[SerializeField] private GameObject canRevive;

	[Header("Trader UI")]
	[SerializeField] private GameObject traderUI;
	[SerializeField] private Text nbPlayerCoinText;
	[SerializeField] private TraderSlot[] traderSlots = new TraderSlot[4];

	[Header("Stats UI")]
	[SerializeField] private GameObject statsUI;
	[SerializeField] private StatsText stats;
	
	[Header("Inventory UI")]
	[SerializeField] private GameObject inventoryUI;
	[SerializeField] private Image Slot1;
	[SerializeField] private Image Slot2;
	[SerializeField] private Image Slot3;

	[Header("Skills UI")]
	[SerializeField] private GameObject skillsUI;

	// Fonction appelée quand le joueur rejoint le serveur
	private void Start()
	{
		// Désactivation de tout les UI sauf waitPlayerUI et l'affichage de l'ip du serveur
		waitPlayerUI.SetActive(true);
		ipServerUI.SetActive(true);

		timerUI.SetActive(false);
		reanimationUI.SetActive(false);
		canRevive.SetActive(false);
		traderUI.SetActive(false);

		ChangeInGameUI(false);

		// On ajoute la fonction HandleGameStateChange à l'event du GameManager qui détecte les changements des états de la partie
		GameManager.instance.EventGameStateChanged += HandleGameStateChange;
	}

	// Activation/Désactivation des différents UI quand on change de GameState
	private void HandleGameStateChange(GameUtils.GameStates currentGameState, GameUtils.GameStates previousGameState)
	{
		// Désactivation des différents UI car on change de state
		switch (previousGameState)
		{
			case GameUtils.GameStates.WAIT_PLAYER:
				waitPlayerUI.SetActive(false);
				ipServerUI.SetActive(false);
				break;
			case GameUtils.GameStates.TIMER:
				timerUI.SetActive(false);
				break;
			case GameUtils.GameStates.PLAY:
				ChangeInGameUI(false);
				break;
			default:
				break;
		}

		switch (currentGameState)
		{
			case GameUtils.GameStates.WAIT_PLAYER:
				waitPlayerUI.SetActive(true);
				ipServerUI.SetActive(true);
				break;
			case GameUtils.GameStates.TIMER:
				timerUI.SetActive(true);
				break;
			case GameUtils.GameStates.PLAY:
				ChangeInGameUI(true);
				break;
			default:
				break;
		}
	}

	public void UpdateTimer(int timeInSeconds)
	{
		// On update le temps du timer à l'aide de la variable timeInSeconds
		timerText.text = timeInSeconds.ToString() + " seconde(s)";
	}
	public void UpdateIpServer(string serverIp)
	{
		// On update l'ip du serveur
		ipServerText.text = serverIp;
	}

    #region Réanimation
    public void ChangeReanimationStateUI(bool state, string resuscitatorPseudo = "")
	{
		reanimationUI.SetActive(state);

		if (state)
			reanimationText.text = resuscitatorPseudo + " est entrain de vous réanimer";
	}
	public void ChangeCanReviveStateUI(bool state)
    {
		canRevive.SetActive(state);
	}
	#endregion

	#region Marchand
	public bool GetTraderUIState { get => traderUI.activeSelf; }
	public void OpenTraderUI(SyncList<Item> items) {
		ChangeInGameUI(false);

		traderUI.SetActive(true);
		nbPlayerCoinText.text = GameManager.instance.GetLocalPlayer.GetComponent<Inventory>().getCoins().ToString();

		// Actualisation de l'UI
        for (int i = 0; i < traderSlots.Length; i++)
        {
			traderSlots[i].priceText.text = items[i].cost.ToString();
			traderSlots[i].descText.text = items[i].description.ToString();
			traderSlots[i].iconImage.sprite = items[i].GFX.GetComponent<SpriteRenderer>().sprite;
        }
	}
	public void CloseTraderUI() {
		traderUI.SetActive(false);

		ChangeInGameUI(true);
	}
	public void TraderBuyButtonClick(int slotID)
    {
		PlayerController pController = GameManager.instance.GetLocalPlayer.GetComponent<PlayerController>();
		pController.callBuyItem(slotID); 
		nbPlayerCoinText.text = GameManager.instance.GetLocalPlayer.GetComponent<Inventory>().getCoins().ToString();
		Debug.Log("Tentative d'achat d'un item ! ");
    }
	public void TraderUpdatePlayerCoins(int nbCoins)
    {
		nbPlayerCoinText.text = nbCoins.ToString();
	}
    #endregion

    #region Stats
	public void UpdateStatsText(PlayerStats pStats)
    {
		if (!pStats.GetHealth) return;

		stats.health.text = pStats.GetHealth.CurrentHealth.ToString() + " / " + pStats.GetHealth.MaxHealth.ToString();
		stats.physicalAttack.text = pStats.m_physicalAttack.ToString();
		stats.elementaryAttack.text = pStats.m_elementaryAttack.ToString();
		stats.defense.text = pStats.m_defense.ToString();
		stats.elementaryDefense.text = pStats.m_elementaryDefense.ToString();
		stats.speed.text = pStats.m_speed.ToString();
		stats.cooldownReduction.text = pStats.m_cooldownReduction.ToString();
		stats.vampirism.text = pStats.m_vampirism.ToString();
	}
	#endregion

	#region Ability Tree
	public void AbilityTreeChangeState(bool state)
    {
		ChangeInGameUI(!state);
	}

	#endregion

	#region Inventaire
	public bool GetInventoryState { get => inventoryUI.activeSelf; }
	public void ChangeInventoryInterface(NetworkIdentity caller)
    {
        if (GetInventoryState)
		{
			Item[] pInventory = caller.GetComponent<Inventory>().getItemInventory();

            if (pInventory[0])
				Slot1.sprite = pInventory[0].GFX.GetComponent<SpriteRenderer>().sprite;
			if (pInventory[1])
				Slot2.sprite = pInventory[1].GFX.GetComponent<SpriteRenderer>().sprite;
			if (pInventory[2])
				Slot3.sprite = pInventory[2].GFX.GetComponent<SpriteRenderer>().sprite;
		}
    }
	public void ChangeInventoryState()
    {
		inventoryUI.SetActive(!GetInventoryState);
	}

	#endregion

	private void ChangeInGameUI(bool state)
    {
		skillsUI.SetActive(state);
		statsUI.SetActive(state);
		inventoryUI.SetActive(state);
	}

	protected override void OnDestroy()
	{
		if (GameManager.instance)
			GameManager.instance.EventGameStateChanged -= HandleGameStateChange;
		base.OnDestroy();
	}
}
