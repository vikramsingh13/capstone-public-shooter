using UnityEngine;

[System.Serializable]
public abstract class EntityStats : MonoBehaviour
{
    public float HitPoints = 100;

    public Stat MovementSpeed;
    public Stat JumpHeight;

    //Initialize all stats
    protected void Start()
    {
        MovementSpeed.Init();
        JumpHeight.Init();
    }
}
