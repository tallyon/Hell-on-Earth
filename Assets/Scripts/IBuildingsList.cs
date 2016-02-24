using System.Collections.Generic;

namespace Assets.Scripts
{
    interface IBuildingsList
    {
        List<Building> BuildingsList { get; set; }

        int RegisterBuilding(Building building);
    }
}
