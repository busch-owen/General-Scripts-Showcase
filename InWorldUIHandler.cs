using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InWorldUIHandler : MonoBehaviour
{
    #region So many fun variables I'm gonna have to assign later :D

    [SerializeField]
    TMP_Text pointsText;

    [SerializeField] 
    Transform pointsTextTargetPos;

    [SerializeField] 
    Transform pointsTextDefaultPos;
    
    [SerializeField]
    Transform warehousePickupPromptText;

    [SerializeField]
    Transform warehousePickupDefaultPos;

    [SerializeField]
    Transform warehousePickupTargetPos;

    [SerializeField]
    TMP_Text levelTimerText;

    [SerializeField]
    TMP_Text quotaText;

    [SerializeField]
    TMP_Text warehouseRefillText;

    [SerializeField]
    TMP_Text packagesAmountText;

    [SerializeField]
    TMP_Text runLevelText;

    [SerializeField]
    Image runLevelProgressBarFill;

    [SerializeField]
    Gradient levelProgressFillGradient;

    #endregion

    #region Points Text Updates

    public void UpdatePointsText(int score)
    {
        pointsText.transform.position = Vector3.Lerp(pointsText.transform.position, pointsTextTargetPos.position, 4f * Time.deltaTime);
        pointsText.text = "Accuracy: " + score;
    }

    public void ReturnPointsText()
    {
        pointsText.transform.position = pointsTextDefaultPos.position;
    }

    #endregion

    #region Player Stats UI Handling

    public void UpdateQuotaText(int amount, int quota)
    {
        quotaText.text = $"Quota: {amount}/{quota}";
    }

    public void UpdatePackageAmountText(int amount)
    {
        packagesAmountText.text = ": " + amount;
    }

    public void UpdateLevelStats(int level, float levelProgress)
    {
        runLevelProgressBarFill.type = Image.Type.Filled;
        runLevelText.text = "Level: " + level;
        runLevelProgressBarFill.color = levelProgressFillGradient.Evaluate(runLevelProgressBarFill.fillAmount);
        runLevelProgressBarFill.fillAmount = levelProgress;
    }

    #endregion

    #region All of the Warehouse Stuff

    public void LerpPickupTextIntoPos()
    {
        warehousePickupPromptText.position = Vector3.Lerp(warehousePickupPromptText.position, warehousePickupTargetPos.position, 3f * Time.deltaTime);
    }
    public void LerpPickupTextOutOfPos()
    {
        warehousePickupPromptText.position = Vector3.Lerp(warehousePickupPromptText.position, warehousePickupDefaultPos.position, 3f * Time.deltaTime);
    }

    public void HandleWarehouseRefillText(bool empty)
    {
        Animator anim = warehouseRefillText.GetComponent<Animator>();
        anim.SetBool("empty", empty);
    }

    #endregion

    #region Game Loop UI

    public void RefreshGameTimer(int timer, string AMorPM)
    {
        levelTimerText.text = timer + " " + AMorPM;
    }

    #endregion
}
