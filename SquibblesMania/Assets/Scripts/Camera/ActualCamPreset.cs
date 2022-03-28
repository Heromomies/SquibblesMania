using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActualCamPreset
{
    public enum Team
    {
        TeamOne,
        TeamTwo
    };

    public static Team CamPresetTeam()
    {
        if (GameManager.Instance.actualCamPreset.presetNumber == 1 || GameManager.Instance.actualCamPreset.presetNumber == 2)
        {
            return Team.TeamOne;
        }

        if (GameManager.Instance.actualCamPreset.presetNumber == 3 || GameManager.Instance.actualCamPreset.presetNumber == 4)
        {
            return Team.TeamTwo;
        }

        return CamPresetTeam();
    }

}
