using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUIManager : MonoBehaviour
{
    [SerializeField] private GameObject questPanel;

    [SerializeField] private Button declineQuestButton;
    [SerializeField] private Button acceptQuestButton;

    [SerializeField] private Button cancelQuestButton;
    [SerializeField] private Button finishQuestButton;

    [SerializeField] private TextMeshProUGUI questName;
    [SerializeField] private TextMeshProUGUI questDescription;
    [SerializeField] private TextMeshProUGUI questObjectives;

    private Action questNegativeCallback;
    private Action questPositiveCallback;
    private Action closeAction;

    public void OfferQuest(QuestInfoSO quest, Action questDeclinedCallback, Action questAcceptedCallback)
    {
        this.questNegativeCallback = questDeclinedCallback;
        this.questPositiveCallback = questAcceptedCallback;
        closeAction = questDeclinedCallback;

        DisplayQuest(quest);

        declineQuestButton.gameObject.SetActive(true);
        acceptQuestButton.gameObject.SetActive(true);

        cancelQuestButton.gameObject.SetActive(false);
        finishQuestButton.gameObject.SetActive(false);
    }
    
    public void DisplayActiveQuest(QuestInfoSO quest, bool canBeFinished, Action questCanceledCallback, Action questFinishedCallback)
    {
        this.questNegativeCallback = questCanceledCallback;
        this.questPositiveCallback = questFinishedCallback;
        closeAction = null;

        DisplayQuest(quest);

        cancelQuestButton.gameObject.SetActive(true);
        finishQuestButton.gameObject.SetActive(true);

        declineQuestButton.gameObject.SetActive(false);
        acceptQuestButton.gameObject.SetActive(false);

        finishQuestButton.interactable = canBeFinished;
    }

    private void DisplayQuest(QuestInfoSO quest)
    {
        ShowQuestUI();

        questName.text = quest.displayName;
        questDescription.text = quest.description;
        questObjectives.text = quest.objectives;
    }

    public void QuestDecision(bool decision)
    {
        //HideQuestUI();
        if (decision)
        {
            questPositiveCallback();
        } else
        {
            questNegativeCallback();
        }
    }

    public void HideQuestUI()
    {
        questPanel.SetActive(false);
    }
    
    private void ShowQuestUI()
    {
        questPanel.SetActive(true);
    }

    public void CloseButtonAction()
    {
        HideQuestUI(); 
        closeAction?.Invoke();
    }
}
