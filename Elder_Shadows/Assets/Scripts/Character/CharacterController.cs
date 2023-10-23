using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public Joystick joystick;
    [SerializeField] private float movespeed;
    public CharacterData characterData;

    void Update()
    {
        transform.position += new Vector3(joystick.Horizontal, joystick.Vertical, transform.position.z) * movespeed * Time.deltaTime;
    }
}
