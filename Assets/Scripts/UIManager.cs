using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Threading;

public class UIManager : MonoBehaviour
{
    public GameObject damageTextPrefab;
    public GameObject healthTextPrefab;
    public GameObject upgradeTextPrefab;

    public Canvas gameCanvas;

    private void Awake()
    {
        gameCanvas = FindAnyObjectByType<Canvas>();
    }

    private void OnEnable()
    {
        CharacterEvents.characterDamaged += (CharacterTookDamage);
        CharacterEvents.characterHealed += (CharacterHealed);
        CharacterEvents.pickaxeUpgraded += (PickaxeUpgraded);
        CharacterEvents.barrierImmune += (BarrierImmune);
        CharacterEvents.magicGemObtained += (MagicGemObtained);
    }

    private void OnDisable()
    {
        CharacterEvents.characterDamaged -= (CharacterTookDamage);
        CharacterEvents.characterHealed -= (CharacterHealed);
        CharacterEvents.pickaxeUpgraded -= (PickaxeUpgraded);
        CharacterEvents.barrierImmune -= (BarrierImmune);
        CharacterEvents.magicGemObtained -= (MagicGemObtained);
    }


    public void CharacterTookDamage(GameObject character, int damageReceived)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(damageTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = damageReceived.ToString();
    }

    public void CharacterHealed(GameObject character, int healthRestored)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(healthTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        tmpText.text = healthRestored.ToString();
    }

    public void BarrierImmune(GameObject character, int health)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(upgradeTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        if (health == 15)
        {
            tmpText.text = "Diperlukan Bronze Pickaxe";
        }
        else if (health == 25)
        {
            tmpText.text = "Diperlukan Iron Pickaxe";
        }
        else if (health == 35)
        {
            tmpText.text = "Diperlukan Gold Pickaxe";
        }
    }

    public void MagicGemObtained(GameObject character, int magicGem)
    {
        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);
        TMP_Text tmpText = Instantiate(upgradeTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();
        if (magicGem == 1)
        {
            tmpText.text = "Air Jump Enabled";
        }
        else if (magicGem == 2)
        {
            tmpText.text = "Dash Enabled";
        }
        else if (magicGem == 3)
        {
            tmpText.text = "Wall Slide Enabled";
        }
    }

    public void PickaxeUpgraded(GameObject character, int baseAttack, int damageUpgrade, int playerDamage)
    {

        Vector3 spawnPosition = Camera.main.WorldToScreenPoint(character.transform.position);

        TMP_Text tmpText = Instantiate(upgradeTextPrefab, spawnPosition, Quaternion.identity, gameCanvas.transform).GetComponent<TMP_Text>();

        if (playerDamage == baseAttack + damageUpgrade)
            tmpText.text = "Stone Pickaxe -> Bronze Pickaxe";
        else if (playerDamage == baseAttack + (damageUpgrade * 2))
        {
            tmpText.text = "Bronze Pickaxe -> Iron Pickaxe";
        }
        else if (playerDamage == baseAttack + (damageUpgrade * 3))
        {
            tmpText.text = "Iron Pickaxe -> Gold Pickaxe";
        }
    }

    public void OnExitGame(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log(this.name + " : " + this.GetType() + " : " + System.Reflection.MethodBase.GetCurrentMethod().Name);
            SceneManager.LoadSceneAsync(0);
        }
    }
}
