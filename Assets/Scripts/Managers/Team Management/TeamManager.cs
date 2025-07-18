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
    }

    public static void InitializeTeams(int _TeamCount, List<int> _Factions, decimal _StartingCash) 
    {
        // Initializing a List with a max size of the number of teams + 1 so we can account for the neutral team.
        Instance.TeamData = new List<TeamData>(_TeamCount + 1);

        // Adding the neutral team to the list
        Instance.TeamData[0] = new TeamData(-1, 0, 0, false);

        for(int index = 0; index < _Factions.Count; index++)
        {
            Instance.TeamData[index + 1] = new TeamData(index == 0 ? 0 : -1, index, _Factions[index], index == 0 ? true : false);
            Instance.TeamData[index + 1].UpdateCash(_StartingCash);
        } 
    }

}
