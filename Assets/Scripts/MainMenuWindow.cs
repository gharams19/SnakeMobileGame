using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
public class MainMenuWindow : MonoBehaviour
{
    private enum Sub {
        Main,
        HowToPlay,
    }
    private void Awake() {
        

        transform.Find("howToPlaySub").Find("howToPlay").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        transform.Find("howToPlaySub").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        transform.Find("mainSub").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        transform.Find("mainSub").Find("playBtn").GetComponent<Button_UI>().ClickFunc = () => Loader.Load(Loader.Scene.GameScene);
        transform.Find("mainSub").Find("quitBtn").GetComponent<Button_UI>().ClickFunc = () => Application.Quit();

        transform.Find("mainSub").Find("howToPlayBtn").GetComponent<Button_UI>().ClickFunc = () => ShowSub(Sub.HowToPlay);
        transform.Find("howToPlaySub").Find("howToPlay").Find("backBtn").GetComponent<Button_UI>().ClickFunc = () => ShowSub(Sub.Main);

        ShowSub(Sub.Main);
    }
    private void ShowSub(Sub sub) {
        transform.Find("mainSub").gameObject.SetActive(false);
        transform.Find("howToPlaySub").gameObject.SetActive(false);

        switch(sub) {
            case Sub.Main:  transform.Find("mainSub").gameObject.SetActive(true); break;
            case Sub.HowToPlay:  transform.Find("howToPlaySub").gameObject.SetActive(true); break;

        }

    }
}
