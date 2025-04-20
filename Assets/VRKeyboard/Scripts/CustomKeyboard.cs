using TMPro;
using UnityEngine;

namespace VRKeyboard
{

    public class CustomKeyboard : MonoBehaviour
    {
        private TMP_InputField tmpInputField, previousInputField;

        [SerializeField] private GameObject keyboard, set1, set2, set3;

        [SerializeField] private AudioSource audioSource;

        [SerializeField] private Animator animator;

        public static CustomKeyboard Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            keyboard.SetActive(false);
        }

        public void Delete()
        {
            int start = Mathf.Min(tmpInputField.selectionAnchorPosition, tmpInputField.selectionFocusPosition);
            int end = Mathf.Max(tmpInputField.selectionAnchorPosition, tmpInputField.selectionFocusPosition);

            if (start == end)
            {
                if (start == 0) return;
                start -= 1;
            }

            tmpInputField.text = tmpInputField.text.Remove(start, end - start);
            tmpInputField.caretPosition = start;

            Canvas.ForceUpdateCanvases();
        }

        public void WriteChar(string character)
        {
            if (tmpInputField.selectionFocusPosition - tmpInputField.selectionAnchorPosition != 0) Delete();

            int caretPos = tmpInputField.caretPosition;
            tmpInputField.text = tmpInputField.text.Insert(caretPos, character);
            tmpInputField.caretPosition = caretPos + 1;
        }

        public void EnableKeyboard(TMP_InputField tmpInputField)
        {
            keyboard.SetActive(true);
            this.tmpInputField = tmpInputField;
            if (tmpInputField == previousInputField) return;
            SetKeyboardPosition();
            animator.Play("Keyboard", 0, 0.0f);
            previousInputField = tmpInputField;
        }

        private void SetKeyboardPosition()
        {
            keyboard.transform.parent.position = Camera.main.transform.position +
                                                 Vector3.Normalize(Vector3.ProjectOnPlane(Camera.main.transform.forward,
                                                     Vector3.up)) * 0.5f;
            keyboard.transform.parent.LookAt(Camera.main.transform, Vector3.up);
            keyboard.transform.parent.eulerAngles = new Vector3(20, keyboard.transform.parent.eulerAngles.y - 180,
                keyboard.transform.parent.eulerAngles.z);
            keyboard.transform.parent.position -= new Vector3(0, 0.3f, 0);
        }

        public void Close()
        {
            animator.Play("Keyboard Reverse");
            previousInputField = null;
            Invoke(nameof(DisableGameobject), 0.45f);
        }

        private void DisableGameobject()
        {
            keyboard.SetActive(false);
            set1.SetActive(true);
            set2.SetActive(false);
            set3.SetActive(false);
        }

        public void PlaySound() => audioSource.Play();
    }
}
