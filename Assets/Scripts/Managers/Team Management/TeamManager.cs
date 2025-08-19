using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;

    public List<TeamData> TeamData = new List<TeamData>();

    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        //InitializeTeams(2, new List<int> { 1, 2 }, 2500);

        DeployInitialUnits();
    }

    public static void InitializeTeams(int _TeamCount, List<int> _Factions, decimal _StartingCash) 
    {
        // Initializing a List with a max size of the number of teams + 1 so we can account for the neutral team.
        Instance.TeamData = new List<TeamData>(_TeamCount + 1);

        // Adding the neutral team to the list
        Instance.TeamData.Add(new TeamData(-1, 0, 0, false));

        for(int index = 0; index < _Factions.Count; index++)
        {
            Instance.TeamData.Add(new TeamData(index == 0 ? 0 : -1, index, _Factions[index], index == 0 ? true : false));
            Instance.TeamData[index + 1].UpdateCash(_StartingCash);
        } 
    }

    public static void DeployInitialUnits()
    {
        // Now for each team, we are going to spawn in their command structure
        for(int index = 0; index < Instance.TeamData.Count; index++)
        {
            // TODO: We need to create objects which will represent the possible starting locations for a team.
            // TODO: There should be a structure which will allow factions to easily dictate their variants for units, structures, etc.

            // TEMPORARY: Generating a position within a 50-unit radius to be the starting location.
            Vector3 startingLocation;

            startingLocation = Random.insideUnitSphere * 50f;
            startingLocation.y = 0f;

            // Spawn in the Command Centre
            EntityManager.Instance.SpawnEntity(0, startingLocation, out bool success);
        }
    }

    public static decimal GetTeamCash(int _TeamIndex)
    {
        return Instance.TeamData[_TeamIndex].Resources.Cash;
    }

    public static void UpdateTeamCash(int _TeamIndex, decimal _Cash)
    {
        Instance.TeamData[_TeamIndex].UpdateCash(_Cash);
    }
}
