using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamData
{
    public int PlayerIndex { get { return _playerIndex; } }
    private int _playerIndex;

    public int TeamIndex { get { return _teamIndex; } }
    private int _teamIndex;

    public int FactionID { get { return _factionId; } }
    private int _factionId;                             // 0 - Neutral, 1 - Humans, 2 - Robots.

    public bool PlayerControlled { get { return _isPlayerControlled; } }
    private bool _isPlayerControlled;

    public TeamResources Resources;

    public TeamData(int _PlayerIndex, int _TeamIndex, int _FactionID, bool _IsPlayerControlled)
    {
        _playerIndex = _PlayerIndex;
        _teamIndex = _TeamIndex;
        _factionId = _FactionID;

        _isPlayerControlled = _IsPlayerControlled;

        Resources = new TeamResources();
    }

    public void UpdateCash(decimal _Amount)
    {
        Resources.UpdateCash(_Amount);
    }
}

public class TeamResources
{
    public decimal Cash { get { return _cash; } }
    private decimal _cash;

    public TeamResources()
    {
        _cash = 0;
    }

    public void UpdateCash(decimal _Amount)
    {
        _cash += _Amount;
    }
}
