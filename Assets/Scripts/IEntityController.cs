public interface IEntityController
{
    /// <summary>
    /// Receive damage equal to /dmg/. Returns 0 on success, -1 when failed to receive damage.
    /// </summary>
    /// <param name="dmg"></param>
    /// <returns></returns>
    int OnReceiveDamage(int dmg);
}
