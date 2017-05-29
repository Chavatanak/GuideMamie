using UnityEngine;
using System.Collections;

public class ActionObject : MonoBehaviour
{

    protected RaycastHit hit;
    protected Ray ray;
    protected LayerMask mask;

    private Vector3 _startPosition;
    private Vector3 _endPosition;
    private float timeTakenDuringLerp = .9f;
    private float _timeStartedLerping;
    private bool _isLerping;

    #region Lifecycle
    protected void Start ()
    {
        ray.origin = transform.position - Vector3.up * .6f;
        ray.direction = Vector3.up;
        mask = 1 << LayerMask.NameToLayer("cube");

        // initialisation animation
        transform.position = new Vector3(transform.position.x, transform.position.y + .5f, transform.position.z);
        _startPosition = transform.position;
        _endPosition = new Vector3(transform.position.x, transform.position.y - .5f, transform.position.z);

        _isLerping = true;
        _timeStartedLerping = Time.time;
    }

    // animation
    void FixedUpdate() {
        if (_isLerping)
        {
            float timeSinceStarted = Time.time - _timeStartedLerping;
            float percentageComplete = timeSinceStarted / timeTakenDuringLerp;
            transform.position = Vector3.Slerp(_startPosition, _endPosition, Easing.Elastic.InOut(percentageComplete));
            if (percentageComplete >= 1.0f)
                _isLerping = false;
        }
    }
    #endregion

    #region Methods
    // vérifie s'il y a un cube sur l'objet
    public Transform checkIn()
    {
        if (Physics.Raycast(ray, out hit, 1f, mask))
            return transform;
        else
            return null;
    }
    #endregion
}
