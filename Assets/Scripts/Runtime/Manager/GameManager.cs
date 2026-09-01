using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region 프로퍼티
    public bool IsPaused { get; private set; }
    #endregion

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        CPrint.Log("게임 일시정지");
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        CPrint.Log("게임 재개");
    }
}