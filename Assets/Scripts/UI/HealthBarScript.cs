using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour
{
    public Sprite FullHeart;
    public Sprite EmptyHeart;
    public Image[] Hearts;
    //Start is called before the first frame update
    #region Singleton Things
    private static HealthBarScript _instance;
    public static HealthBarScript Instance { get { return _instance; } }
    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(this.gameObject);
        } else {
            _instance = this;
        }
    }
    #endregion

    // Update is called once per frame
    void Start() {
        UpdateHealthBar(PlayerEntity.Instance);     
    }
    public void UpdateHealthBar(iHealthInterface iHealth) {
        int offset = Mathf.FloorToInt(Hearts.Length / 2);
        int offsetHealth = offset - Mathf.FloorToInt(iHealth.MaxHealth_/2);
        for (int i = 0; i < Hearts.Length; i++) {
            if (i - offsetHealth < iHealth.MaxHealth_ && i - offsetHealth >= 0) {
                Hearts[i].enabled = true;
            } else {
                Hearts[i].enabled = false;
            }
            if (i - offsetHealth < iHealth.Health_) {
                Hearts[i].sprite = FullHeart;
            } else {
                Hearts[i].sprite = EmptyHeart;
            }
        }
    }
}
