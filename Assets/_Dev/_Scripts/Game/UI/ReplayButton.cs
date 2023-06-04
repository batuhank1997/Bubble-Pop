using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace _01_Scripts.Game.UI
{
    public class ReplayButton : MonoBehaviour
    {
        void Start() => GetComponent<Button>().onClick.AddListener(ReloadCurrentScene);
        void ReloadCurrentScene() => SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
