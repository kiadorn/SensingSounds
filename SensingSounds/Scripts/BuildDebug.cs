using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CATAHL
{
    /// <summary>
    /// Updates text rows in ScreenSpace canvas. Used as a debugging tool when testing on Android builds as a replacement to <see cref="Debug"/>.
    /// </summary>
    public class BuildDebug : MonoBehaviour
    {
        /// <summary>
        /// Collection of debug lines.
        /// </summary>
        private Dictionary<string, GameObject> debugLines = new Dictionary<string, GameObject>();

        /// <summary>
        /// Singleton used internally for the static <see cref="BuildDebug.Log(string, object)"/> function.
        /// </summary>
        private static BuildDebug instance;

        /// <summary>
        /// Prefab with <see cref="TextMeshProUGUI"/> and <see cref="BuildDebugRow"/> for updating the text.
        /// </summary>
        [SerializeField]
        private GameObject buildDebugRow = null;

        /// <summary>
        /// Parent to all <see cref="BuildDebugRow"/> instances.
        /// </summary>
        [SerializeField]
        private GameObject listOfDebugLines = null;

        /// <summary>
        /// Dropdown for selecting which <see cref="BuildDebugRow"/> to show or hide.
        /// </summary>
        [SerializeField]
        private TMP_Dropdown debugDropdown = null;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            if (!Debug.isDebugBuild)
                debugDropdown.gameObject.SetActive(false);
        }

        /// <summary>
        /// Works similarly to <see cref="Debug.Log(object)"/>, but shows up in screen space canvas.
        /// </summary>
        /// <param name="title">Title shown prior to the value, also used to make sure the row is unique.</param>
        /// <param name="value">Any value that is expected to update overtime.</param>
        public static void Log(string title, object value)
        {
            if (!Debug.isDebugBuild)
                return;

            GameObject debugRowObject = null;

            if (!instance.debugLines.ContainsKey(title))
            {
                debugRowObject = instance.AddNewRow(title);
            }
            else
            {
                debugRowObject = instance.debugLines[title];
            }

            BuildDebugRow debugRow = debugRowObject.GetComponent<BuildDebugRow>();
            debugRow.SetText(title + value.ToString());
        }

        /// <summary>
        /// Switches the active status of a <see cref="buildDebugRow"/> corresponding the selected dropdown value.
        /// </summary>
        public void SetTextStatus()
        {
            GameObject selectedRow = listOfDebugLines.transform.GetChild(debugDropdown.value).gameObject;
            selectedRow.SetActive(!selectedRow.activeSelf); 
        }

        /// <summary>
        /// Add a new row to the collection of <see cref="buildDebugRow"/> instances.
        /// </summary>
        /// <param name="title">Title shown prior to the value, also used to make sure the row is unique.</param>
        /// <returns></returns>
        private GameObject AddNewRow(string title)
        {
            GameObject newDebugRow = Instantiate(buildDebugRow, listOfDebugLines.transform);
            debugLines.Add(title, newDebugRow);
            debugDropdown.ClearOptions();
            debugDropdown.AddOptions(new List<string>(debugLines.Keys));
            newDebugRow.SetActive(false);
            return newDebugRow;
        }
    }
}
