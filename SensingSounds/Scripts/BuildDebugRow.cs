using TMPro;
using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Simple object on ScreenSpace canvas showing the given title and value from <see cref="BuildDebug"/>.
    /// </summary>
    public class BuildDebugRow : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI fullText = null;

        public void SetText(string text)
        {
            if (!Debug.isDebugBuild)
                return;

            fullText.text = text;
        }
    }
}
