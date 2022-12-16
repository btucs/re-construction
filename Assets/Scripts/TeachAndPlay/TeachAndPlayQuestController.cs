using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachAndPlayQuestController : MonoBehaviour
{
    public MainQuestSO questData;
    private GameController controller; 

    private void Initialize()
    {
        controller = GameController.GetInstance();
    }

    public bool IsQuestActive()
    {
        return GameController.GetInstance().gameState.profileData.IsQuestActive(questData);
    }

    public void AddQuestInstance()
    {
        GameController.GetInstance().gameState.profileData.AddActiveQuest(questData);
        GameController.GetInstance().SaveGame();
    }

}
