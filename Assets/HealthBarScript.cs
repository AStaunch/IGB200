using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public Image[] Hearts;
    public GameObject target;
    //Start is called before the first frame update
    void Start() {
        target = GameObject.FindGameObjectWithTag("Player");
        UpdateHealthBar();
    }

    // Update is called once per frame
    void Update() {
        UpdateHealthBar();
    }

    void UpdateHealthBar() {
        for (int i = 0; i < Hearts.Length; i++) {
            if (i < target.GetComponent<PlayerEntity>().health) {
                Hearts[i].sprite = FullHeart;
            } else {
                Hearts[i].sprite = EmptyHeart;
            }

            if (i < target.GetComponent<PlayerEntity>().maxHealth) {
                Hearts[i].enabled = true;
            } else {
                Hearts[i].enabled = false;
            }
        }
    }
}
