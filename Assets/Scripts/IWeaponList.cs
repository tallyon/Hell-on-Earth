using System.Collections.Generic;

namespace Assets.Scripts
{
    interface IWeaponList
    {
        List<Weapon> WeaponsList { get; set; }

        int RegisterWeapon(Weapon weapon);
    }
}
