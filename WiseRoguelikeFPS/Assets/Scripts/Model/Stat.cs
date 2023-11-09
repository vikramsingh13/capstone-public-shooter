using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField] private float baseValue;

    [SerializeField]  private float currentValue;

    //Initialize sets current value to base value, gets called at start of game
    public void Init()
    {
        currentValue = baseValue;
    }

    public float GetBaseValue()
    {
        return baseValue;
    }

    public float GetCurrentValue ()
    {
        return currentValue;
    }

    public void EditStatPercent(float modifier)
    {
        currentValue += baseValue * modifier;
    }

    public void EditStat(float modifier)
    {
        currentValue += modifier;
    }
}
