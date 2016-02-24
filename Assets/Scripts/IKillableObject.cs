namespace Assets.Scripts
{
    interface IKillableObject
    {
        int CurrentHP { get; set; }
        int MaxHP { get; set; }
        bool IsAlive { get; set; }
        bool IsSpawned { get; set; }

        void OnReceiveDamage(int dmg);
        void Dead();
    }
}
