using UnityEngine;

public class BatInputController : MonoBehaviour
{
    [SerializeField]
    private GameObject _leftWing;
    [SerializeField]
    private GameObject _leftWingHit;
    [SerializeField]
    private GameObject _rightWing;
    [SerializeField]
    private GameObject _rightWingHit;

    private void Update()
    {
        // still using osu buttons for now
        var leftPressed = Input.GetKey(KeyCode.Z);
        var rightPressed = Input.GetKey(KeyCode.X);
        
        _leftWing.SetActive(!leftPressed);
        _leftWingHit.SetActive(leftPressed);

        _rightWing.SetActive(!rightPressed);
        _rightWingHit.SetActive(rightPressed);
    }
}
