using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float h;
    private float v;
    private float h_R;
    private float v_R;
    private Character character;
    private int inversionModifier = -1;

    void Awake()
    {
        character = GetComponent<Character>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        GetAxisInput();
        GetButtonInput();
    }

    private void GetAxisInput()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        h_R = Input.GetAxis("R_Horizontal");
        v_R = Input.GetAxis("R_Vertical");

        if (character != null)
        {
            character.Move(h, v);
            character.Rotate(h_R, v_R, inversionModifier);
        }
    }

    private void GetButtonInput()
    {
        float triggerValue = Input.GetAxis("Triggers");
        if (triggerValue < -0.25f)
        {
            character.Shoot();
        }

        if (Input.GetButtonDown("LBumper"))
        {
            GameManager.Instance.AdjustSensitivity(-1);
        }
        else if (Input.GetButtonDown("RBumper"))
        {
            GameManager.Instance.AdjustSensitivity(1);
        }

        if (Input.GetButtonDown("Invert"))
        {
            inversionModifier *= -1;
            GameManager.Instance.UpdateInvertPreference(inversionModifier < 0);
        }

        if (Input.GetButtonDown("Pause"))
        {
            GameManager.Instance.TogglePause();
        }

        if (Input.GetButtonDown("Jump"))
        {
            character.Jump();
        }

        if (Input.GetButtonDown("Light"))
        {
            character.ToggleLight();
        }
    }
}
