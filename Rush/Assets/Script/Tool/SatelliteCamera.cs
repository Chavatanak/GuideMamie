using UnityEngine;
using System.Collections;

public class SatelliteCamera : MonoBehaviour {
	
	public float minDist;
	public float maxDist;

	public float minElevation;
	public float maxElevation;

	public float elevation;
	float targetElevation;

	public float azimutSpeed;
	public float distanceSpeed;
	public float elevationSpeed;

	public float azimutSpeedMouse;
	public float elevationSpeedMouse;

	public float azimutSpeedTouch;
	public float distanceSpeedTouch;
	public float elevationSpeedTouch;

	public Transform target;
	
	public float startAzimut = 45f;
	
	float azimut;
	float targetAzimut;

	float distance;
	float targetDistance;
	
	public float kLerpPos;
	
	Vector3 targetPos;

	Vector3 previousMousePos;
	float previousFingersDist=0;
	bool hasStartedZoom=false;
    

	// Use this for initialization
	void Start () {
		distance = (minDist+maxDist)/2f;
		targetDistance = distance;

		azimut = startAzimut;
		targetAzimut = azimut;

		elevation*=Mathf.PI/180f;
		targetElevation = elevation;

		minElevation*=Mathf.PI/180f;
				maxElevation*=Mathf.PI/180f;

		previousMousePos = Input.mousePosition;
    }
	
	// Update is called once per frame
	void Update () {

		Vector3 dir = Vector3.zero;

		#if UNITY_STANDALONE || UNITY_WEBGL ||UNITY_EDITOR || UNITY_WEBPLAYER
		// à la souris
		if(Input.GetMouseButton(1))
		{
			dir = Input.mousePosition-previousMousePos;
			targetAzimut-=azimutSpeedMouse*Time.deltaTime*dir.x;
			targetElevation+=elevationSpeedMouse*Time.deltaTime*dir.y;
		}

		previousMousePos = Input.mousePosition;

		//au clavier
		if(Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.LeftArrow))
			targetAzimut-=azimutSpeed*Time.deltaTime;
		else if(Input.GetKey(KeyCode.D)  || Input.GetKey(KeyCode.RightArrow))
			targetAzimut+=azimutSpeed*Time.deltaTime;

		if(Input.GetKey(KeyCode.Z) || Input.GetKey(KeyCode.UpArrow))
			targetElevation+=elevationSpeed*Time.deltaTime;
		else if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
			targetElevation-=elevationSpeed*Time.deltaTime;

		targetDistance = Mathf.Clamp(targetDistance- distanceSpeed*Input.GetAxis("Mouse ScrollWheel")*Time.deltaTime,minDist,maxDist);
#endif

#if !UNITY_EDITOR && (UNITY_ANDROID || UNITY_IOS)
		if(Input.touchCount==1 && Input.touches[0].deltaPosition.magnitude>1)
		{
			dir = Input.touches[0].deltaPosition;
			targetAzimut-=azimutSpeedTouch*Time.deltaTime*dir.x;
			targetElevation+=elevationSpeedTouch*Time.deltaTime*dir.y;
		}

		if(Input.touchCount==2)
		{
			float currFingersDist = (Input.touches[0].position-Input.touches[1].position).magnitude;

			if(!hasStartedZoom)
			{
				previousFingersDist = currFingersDist;
				hasStartedZoom = true;
			}

			float deltaFingersDist = currFingersDist-previousFingersDist;

			targetDistance = Mathf.Clamp(targetDistance- distanceSpeedTouch*deltaFingersDist*Time.deltaTime,minDist,maxDist);

			previousFingersDist = currFingersDist;
		}
		else hasStartedZoom = false;
#endif

		targetElevation = Mathf.Clamp(targetElevation,minElevation,maxElevation);

		azimut = Mathf.Lerp(azimut,targetAzimut,Time.deltaTime*kLerpPos);
		elevation = Mathf.Lerp(elevation,targetElevation,Time.deltaTime*kLerpPos);
		distance = Mathf.Lerp(distance,targetDistance,Time.deltaTime*kLerpPos);
		
		Vector3 dirH = new Vector3(Mathf.Cos(azimut),0,Mathf.Sin(azimut));
		
		Vector3 newPos = target.position+dirH*distance*Mathf.Cos(elevation)+Vector3.up*distance*Mathf.Sin(elevation);
		transform.position = newPos;//Vector3.Lerp(transform.position,newPos,Time.deltaTime*kLerpPos);
		transform.LookAt(target);
	}

	#if UNITY_EDITOR ||UNITY_WEBGL || UNITY_DESKTOP || UNITY_WEBPLAYER
	void OnGUI()
	{
    
		GUI.Label(new Rect(10,Screen.height-65,Screen.width,60),"Navigation around a level: ZQSD / Arrows,\nor move the mouse with the right button pressed\n" +
			"Mouse wheel to zoom/unzoom\n");
	}
#endif
}
