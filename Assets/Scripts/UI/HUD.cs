using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*Manages and updates the HUD, which contains your health bar, coins, etc*/

public class HUD : MonoBehaviour
{
    [Header ("Reference")]
    public TextMeshProUGUI bugsMesh;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image[] inventoryItemGraphic;

    [System.NonSerialized] public Sprite blankUI; //The sprite that is shown in the UI when you don't have any items
    private float bugs;
    private float bugsEased;
    private float healthBarWidth;
    private float healthBarWidthEased;
    [System.NonSerialized] public string loadSceneName;
    [System.NonSerialized] public bool resetPlayer;

    private Animator animator;
    
    void Start()
    {
        //Set all bar widths to 1, and also the smooth variables.
        healthBarWidth = 1;
        healthBarWidthEased = healthBarWidth;
        bugs = GameManager.Instance.Bugs;
        bugsEased = bugs;
        blankUI = inventoryItemGraphic[0].GetComponent<Image>().sprite;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        //Update coins text mesh to reflect how many coins the player has! However, we want them to count up.
        bugsMesh.text = Mathf.Round(bugsEased).ToString();
        bugsEased += ((float)GameManager.Instance.Bugs - bugsEased) * Time.deltaTime * 5f;

        if (bugsEased >= bugs)
        {
            animator.SetTrigger("getGem");
            bugs = bugsEased + 1;
        }

        //Controls the width of the health bar based on the player's total health
        healthBarWidth = (float)GameManager.Instance.newPlayer.health / (float)GameManager.Instance.newPlayer.maxHealth;
        healthBarWidthEased += (healthBarWidth - healthBarWidthEased) * Time.deltaTime*2 * healthBarWidthEased;
        healthBar.transform.localScale = new Vector2(healthBarWidthEased, 1);
    }
    public void HealthBarHurt() => animator.SetTrigger("hurt");
    public void SetInventoryImage(Sprite image, int slotNumber) => inventoryItemGraphic[slotNumber].sprite = image;
    public void ShowTitle() => animator.SetBool("showTitle", true);
    public void CoverScreen() => animator.SetTrigger("coverScreen");
}
