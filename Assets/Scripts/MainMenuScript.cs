using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public GameObject TitleCard;
    public GameObject ControlsCard;
    public GameObject[] ControlPGs;
    public GameObject ExitPopup;
    public static int introID = 0;
    public static int menuID = 1;
    public static int gameID = 2;
    public static int endID = 3;

    // Start is called before the first frame update
    private void Start()
    {
        OpenMain();
        CloseExit();
    }
    public static void LOADSCENE(int ID) {
        SceneManager.LoadScene(ID);
    }
    public void INTROSCENE() {
        StartCoroutine(FadeMenu(2, MainMenuScript.introID));
    }
    public void MAINMENU() {
        StartCoroutine(FadeMenu(2, MainMenuScript.menuID));
    }
    public void LAUNCHGAME() {
        StartCoroutine(FadeMenu(2, MainMenuScript.gameID));
    }  
    public static void QUITGAME() {
        Application.Quit();
    }
    public void OpenMain() {
        TitleCard.SetActive(true);
        ControlsCard.SetActive(false);
    }
    public void OpenExit() {
        ExitPopup.SetActive(true);
    }
    public void OpenControls() {
        ControlsCard.SetActive(true);
        TitleCard.SetActive(false);
        FirstPage();
    }
    public void CloseExit() {
        ExitPopup.SetActive(false);
    }
    public void FirstPage() {
        ControlPGs[0].SetActive(true);
        ControlPGs[1].SetActive(false);
    }
    public void SecondPage() {
        ControlPGs[0].SetActive(false);
        ControlPGs[1].SetActive(true);
    }

    public static IEnumerator FadeMenu(float duration, int ID) {
        FadingScript.Instance.FadeScreen(true, duration);
        yield return new WaitForSecondsRealtime(1.5f * duration);
        LOADSCENE(ID);
    }
}
