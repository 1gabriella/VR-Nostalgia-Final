using TMPro;
using UnityEngine;

namespace VRKeyboard
{
    public class InvokeKeyboard : MonoBehaviour
    {
        private TMP_InputField tmpInputField;

        private void Awake()
        {
            tmpInputField = gameObject.GetComponent<TMP_InputField>();
            tmpInputField.onSelect.AddListener(x => CustomKeyboard.Instance.EnableKeyboard(tmpInputField));
        }
    }
}

