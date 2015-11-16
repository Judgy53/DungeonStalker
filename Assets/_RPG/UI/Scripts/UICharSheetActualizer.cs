using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class UICharSheetActualizer : MonoBehaviour
{
    public StatsManager target = null;

    public AttributeUILabel strengthLabel = null;
    public AttributeUILabel defenseLabel = null;
    public AttributeUILabel staminaLabel = null;
    public AttributeUILabel energyLabel = null;

    public Text unspentPointsLabel = null;
    public Text levelLabel = null;

    private CharStats tmpAddedPoint = new CharStats(0u);

    private UICharSheet manager = null;

    void Start()
    {
        GameManager.OnPlayerCreation += OnPlayerCreation;

        manager = GetComponentInParent<UICharSheet>();
        if (manager == null)
        {
            Debug.LogError("No manager found for " + this.name);
            enabled = false;
            return;
        }

        manager.OnMenuStateChange += OnMenuStateChange;
    }

    private void OnPlayerCreation(object sender, EventPlayerCreationArgs e)
    {
        target = e.player.GetComponent<StatsManager>();

        target.OnLevelUp += OnStatsChanged;

        if (strengthLabel != null)
            strengthLabel.Initialize(OnButtonPressed, target.UnspentPoints != 0);
        if (defenseLabel != null)
            defenseLabel.Initialize(OnButtonPressed, target.UnspentPoints != 0);
        if (staminaLabel != null)
            staminaLabel.Initialize(OnButtonPressed, target.UnspentPoints != 0);
        if (energyLabel != null)
            energyLabel.Initialize(OnButtonPressed, target.UnspentPoints != 0);

        tmpAddedPoint.OnStatsChange += OnStatsChanged;
        OnStatsChanged(this, new EventArgs());
    }

    private void OnStatsChanged(object sender, EventArgs args)
    {
        if (strengthLabel != null)
            strengthLabel.ActualizeText((int)target.Stats.Strength + (int)tmpAddedPoint.Strength);
        if (defenseLabel != null)
            defenseLabel.ActualizeText((int)target.Stats.Defense + (int)tmpAddedPoint.Defense);
        if (staminaLabel != null)
            staminaLabel.ActualizeText((int)target.Stats.Stamina + (int)tmpAddedPoint.Stamina);
        if (energyLabel != null)
            energyLabel.ActualizeText((int)target.Stats.Energy + (int)tmpAddedPoint.Energy);

        if (unspentPointsLabel != null)
            unspentPointsLabel.text = "Unspent points : " + target.UnspentPoints;
        if (levelLabel != null)
            levelLabel.text = "Level : " + target.CurrentLevel;

        strengthLabel.minusButton.gameObject.SetActive(tmpAddedPoint.Strength > 0u);
        defenseLabel.minusButton.gameObject.SetActive(tmpAddedPoint.Defense > 0u);
        staminaLabel.minusButton.gameObject.SetActive(tmpAddedPoint.Stamina > 0u);
        energyLabel.minusButton.gameObject.SetActive(tmpAddedPoint.Energy > 0u);

        strengthLabel.plusButton.gameObject.SetActive(target.UnspentPoints != 0);
        defenseLabel.plusButton.gameObject.SetActive(target.UnspentPoints != 0);
        staminaLabel.plusButton.gameObject.SetActive(target.UnspentPoints != 0);
        energyLabel.plusButton.gameObject.SetActive(target.UnspentPoints != 0);
    }

    private void OnButtonPressed(StatType type, int sign)
    {
        int tmpvar = (int)target.UnspentPoints;
        tmpvar -= sign;
        target.UnspentPoints = (uint)tmpvar;

        switch (type)
        {
            case StatType.Stength:
                int tmp = (int)tmpAddedPoint.Strength;
                tmp += sign;
                tmpAddedPoint.Strength = (uint)Mathf.Max(0, tmp);
                break;
            case StatType.Defense:
                tmp = (int)tmpAddedPoint.Defense;
                tmp += sign;
                tmpAddedPoint.Defense = (uint)Mathf.Max(0, tmp);
                break;
            case StatType.Stamina:
                tmp = (int)tmpAddedPoint.Stamina;
                tmp += sign;
                tmpAddedPoint.Stamina = (uint)Mathf.Max(0, tmp);
                break;
            case StatType.Energy:
                tmp = (int)tmpAddedPoint.Energy;
                tmp += sign;
                tmpAddedPoint.Energy = (uint)Mathf.Max(0, tmp);
                break;
        }
    }

    public void ApplyModifications()
    {
        tmpAddedPoint.OnStatsChange -= OnStatsChanged;
        if (target != null)
        {
            target.Stats += tmpAddedPoint;
            target.Stats.FireEvent();
        }
        tmpAddedPoint = new CharStats(0u);
        tmpAddedPoint.OnStatsChange += OnStatsChanged;
    }

    private void OnMenuStateChange(object sender, UIMenuStateChangeArgs args)
    {
        if (args.newState == UIMenuState.Hidden)
            ApplyModifications();
        else
            OnStatsChanged(this, new EventArgs());
    }

    [Serializable]
    public class AttributeUILabel
    {
        public string prefix = "Attribute : ";
        public StatType type = StatType.Stength;

        public Text textUI = null;
        public Button minusButton = null;
        public Button plusButton = null;

        public delegate void DelegateSignature(StatType type, int sign);
        public DelegateSignature buttonsDelegate = null;

        public void Initialize(DelegateSignature func, bool showButtons)
        {
            if (minusButton != null)
            {
                minusButton.onClick.AddListener(MinusCallback);
                minusButton.gameObject.SetActive(false);
            }
            if (plusButton != null)
            {
                plusButton.onClick.AddListener(PlusCallback);
                if (showButtons)
                    plusButton.gameObject.SetActive(false);
            }

            buttonsDelegate = func;
        }

        public void ActualizeText(int value)
        {
            if (textUI != null)
                textUI.text = prefix + value;
        }

        private void MinusCallback()
        {
            if (buttonsDelegate != null)
                buttonsDelegate(type, -1);
        }

        private void PlusCallback()
        {
            if (buttonsDelegate != null)
                buttonsDelegate(type, 1);
        }
    }
}
