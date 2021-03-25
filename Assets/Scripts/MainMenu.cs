using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject scorePanel;
    public GameObject templateRow;
    public GameObject scoreBoard;
    public GameObject inputField;
    public GameObject mainPanel;
    public GameObject auxPanel;
    public void NewGameHandler()
    {
        mainPanel.SetActive(false);
        auxPanel.SetActive(true);
    }

    public void BackHandler()
    {
        mainPanel.SetActive(true);
        scorePanel.SetActive(false);
    }

    public void ScoresHandler()
    {
        scorePanel.SetActive(true);
        mainPanel.SetActive(false);
        CommandInvoker invoker = new CommandInvoker();
        invoker.SetCommand(new ReadDBCommand());
        invoker.RunAll();
        for (int i = 0; i < MetaSceneInformation.names.Count; i++)
        {
            GameObject temp = Instantiate(templateRow);
            temp.GetComponent<Text>().text = MetaSceneInformation.names[i] + temp.GetComponent<Text>().text + MetaSceneInformation.levels[i].ToString();
            temp.transform.SetParent(scoreBoard.transform);
            temp.transform.localScale = Vector3.one;
            temp.GetComponent<RectTransform>().localPosition = new Vector3(temp.GetComponent<RectTransform>().rect.x,
                temp.GetComponent<RectTransform>().rect.y,
                0);
        }
    }

    public void StartGame()
    {
        string potentName = inputField.GetComponent<InputField>().text;
        if (potentName != "")
        {
            MetaSceneInformation.PlayerName = potentName;
        }
        else
        {
            MetaSceneInformation.PlayerName = "anonymous";
        }
        MetaSceneInformation.Level = 1;
        SceneManager.LoadScene("SampleScene");
    }

    public void TutorialHandler()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void ExitHandler()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
