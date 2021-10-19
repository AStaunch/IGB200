using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugBox : MonoBehaviour
{
    public GameObject textBox;
    private Text text;
    private int charsInFirstLine;

    public GameObject bg1;
    public GameObject bg2;
    public GameObject bg3;
    public GameObject bg4;
    public GameObject bg5;
    private Image bg1Img;
    private Image bg2Img;
    private Image bg3Img;
    private Image bg4Img;
    private Image bg5Img;

    private float transSet;
    private float addition;

    public string input;
    private float timer;
    private bool tryTimer = false;

    public List<string> inputs;

    // Start is called before the first frame update
    void Start()
    {
        text = textBox.GetComponent<Text>();
        input = "";
        inputs.Clear();
        //text.text = "poopa";

        bg1Img = bg1.GetComponent<Image>();
        bg2Img = bg2.GetComponent<Image>();
        bg3Img = bg3.GetComponent<Image>();
        bg4Img = bg4.GetComponent<Image>();
        bg5Img = bg5.GetComponent<Image>();

        textBox.SetActive(false);
        bg1.SetActive(false);
        bg2.SetActive(false);
        bg3.SetActive(false);
        bg4.SetActive(false);
        bg5.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (inputs.Count != 0)
        {
            input = inputs[0];
            inputs.RemoveAt(0);

            timer = Time.time + 3f;
            tryTimer = true;
            textBox.SetActive(true);
            text.color = new Color(1f, 1f, 1f, 0.725f);
            bg1Img.color = new Color(0.1981f, 0.1981f, 0.1981f, 0.725f);
            bg2Img.color = new Color(0.1981f, 0.1981f, 0.1981f, 0.725f);
            bg3Img.color = new Color(0.1981f, 0.1981f, 0.1981f, 0.725f);
            bg4Img.color = new Color(0.1981f, 0.1981f, 0.1981f, 0.725f);
            bg5Img.color = new Color(0.1981f, 0.1981f, 0.1981f, 0.725f);
            transSet = 0.725f;
            addition = 0f;

            if (bg1.activeSelf == false)
            {
                bg1.SetActive(true);
                text.text = "//Debug.Log: " + input;
                charsInFirstLine = input.Length + 15;

                input = "";
            }
            else if(bg2.activeSelf == false)
            {
                bg2.SetActive(true);
                text.text = text.text + System.Environment.NewLine + "//Debug.Log: " + input;

                input = "";
            }
            else if (bg3.activeSelf == false)
            {
                bg3.SetActive(true);
                text.text = text.text + System.Environment.NewLine + "//Debug.Log: " + input;

                input = "";
            }
            else if (bg4.activeSelf == false)
            {
                bg4.SetActive(true);
                text.text = text.text + System.Environment.NewLine + "//Debug.Log: " + input;

                input = "";
            }
            else if (bg5.activeSelf == false)
            {
                bg5.SetActive(true);
                text.text = text.text + System.Environment.NewLine + "//Debug.Log: " + input;

                input = "";
            }
            else
            {
                text.text = text.text.Remove(0, charsInFirstLine);
                charsInFirstLine = text.text.Split(':')[0].Length + text.text.Split(':')[1].Length - 10;

                text.text = text.text + System.Environment.NewLine + "//Debug.Log: " + input;
                input = "";
            }
        }

        if (Time.time > timer & tryTimer == true)
        {
            if (Time.time > timer && Time.time <= timer + 0.1f) { transSet = 0.725f - 0.0725f; addition = 0.1f; }
            else if (Time.time > timer + addition && Time.time <= timer + addition + 0.1f) { transSet -= 0.0725f; addition += 0.1f; }

            //if (Time.time > timer && Time.time <= timer + 0.05f) { transSet = 0.725f - 0.03625f; addition = 0.05f; }
            //else if (Time.time > timer + addition && Time.time <= timer + addition + 0.05f) { transSet -= 0.03625f; addition += 0.5f; }

            bg1Img.color = new Color(0.1981f, 0.1981f, 0.1981f, transSet);
            bg2Img.color = new Color(0.1981f, 0.1981f, 0.1981f, transSet);
            bg3Img.color = new Color(0.1981f, 0.1981f, 0.1981f, transSet);
            bg4Img.color = new Color(0.1981f, 0.1981f, 0.1981f, transSet);
            bg5Img.color = new Color(0.1981f, 0.1981f, 0.1981f, transSet);
            text.color = new Color(1, 1, 1, transSet);

            if (Time.time > timer + 1)
            {
                text.text = "";

                textBox.SetActive(false);
                bg1.SetActive(false);
                bg2.SetActive(false);
                bg3.SetActive(false);
                bg4.SetActive(false);
                bg5.SetActive(false);

                tryTimer = false;
            }
        }
    }
}
