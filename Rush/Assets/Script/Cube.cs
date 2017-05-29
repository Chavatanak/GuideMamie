using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public class WarningColEvent : UnityEvent<Transform> { }

public class Cube : MonoBehaviour {

    public WarningColEvent onWarningCol;

    // STATE
    public delegate void State();
    public State move;
    public State anim;

    // Collision
    LayerMask mask;
    LayerMask cubeLayer;
    LayerMask killzone;

    // Spawn
    private float scaleProgress     = 1.0f;

    // Forward 
    public Vector3 moveDirection;
    private Vector3 pivot;
    private Vector3 axis;
    private float newAngle;
    private float oldAngle;
    
    // Fall 
    private Vector3 startPosition;

    // Transition 
    public float transitionDirection;
    public bool canTransition = true;

    // Stopd
    public bool wasWaiting = false;

    // Teleport
    protected Vector3 posToTeleport;
    public bool canTeleport;

    #region Lifecycle
    void Start()
    {
        CubeManager.instance.list.Add(this);
        CubeManager.instance.listenCube(this);

        setModeSpawn(transform.position);
        
        // initialiation mouvement
        moveDirection = transform.forward;
        pivot = transform.position + transform.forward * .5f - transform.up * .5f + transform.right * .5f;

        mask = 1 << LayerMask.NameToLayer("ground");
        cubeLayer = 1 << LayerMask.NameToLayer("cube");
        killzone = 1 << LayerMask.NameToLayer("killzone");
    }
    
    void Awake ()
    {
        onWarningCol = new WarningColEvent();
    }
    
    void Update()
    {
        anim();
    }

    public void destroy()
    {
        Destroy(this.gameObject);
        CubeManager.instance.list.Remove(this);
    }

    // Collision entre deux cube
    void OnTriggerEnter(Collider pCol)
    {
        if (pCol.gameObject.layer == LayerMask.NameToLayer("cube"))
        {
            pCol.gameObject.GetComponent<BoxCollider>().enabled = false;
            GetComponent<BoxCollider>().enabled = false;
            onWarningCol.Invoke(transform);
        }
    }

    #endregion

    #region State Machine

        #region Spawn
        public void setModeSpawn(Vector3 pPosition)
        {
            move = stateSpawn;
            anim = animSpawn;
            transform.position = pPosition;
            canTeleport = false;
            SfxManager.manager.PlaySfx("SFX_Teleportation");
        }

        protected void stateSpawn()
        {
            transform.localScale = new Vector3(1, 1, 1);

            setAxisAndPivot();
            setModeForward();

            if (checkForward(0.51f))
                setModeCollisionGrounds();
        }

        protected void animSpawn()
        {
            scaleProgress = Easing.Elastic.Out(Metronome.progressTime);
            transform.localScale = new Vector3(scaleProgress, scaleProgress, scaleProgress);
        }
        #endregion

        #region Wait
        public void setModeWait()
        {
            move = stateWait;
            anim = animWait;
        }

        protected void stateWait()
        {
            transform.localScale = new Vector3(1, 1, 1);
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));

            setAxisAndPivot();

            oldAngle = 0.0f;
            startPosition = transform.localPosition;

            wasWaiting = true;

            if (checkGround(1f) && checkForward(0.51f))
                setModeCollisionGrounds();
            else
                setModeForward();
        }

        protected void animWait()
        {

        }
        #endregion

        #region Forward
        public void setModeForward()
        {
            move = stateForward;
            anim = animForward;
        }

        protected void stateForward()
        {
            SfxManager.manager.PlayStepSfx();

            transform.localScale = new Vector3(1,1,1);
            transform.RotateAround(pivot, axis, 90.0f - oldAngle);
            snapPosition();
        
            setAxisAndPivot();
        
            oldAngle = 0.0f;
            startPosition = transform.localPosition;
            wasWaiting = false;
            canTeleport = true;

            if (!checkGround(1f))
                setModeFall();

            if (checkGround(1f) && checkForward(0.51f))
                setModeCollisionGrounds();
        }

        protected void animForward()
        {
            newAngle = 90.0f * Easing.Linear(Metronome.progressTime);
            transform.RotateAround(pivot, axis, newAngle - oldAngle);
            oldAngle = newAngle;
        }
        #endregion

        #region CollisionGrounds
        public void setModeCollisionGrounds()
        {
            move = stateCollisionGrounds;
            anim = animCollisionGrounds;
        }

        protected void stateCollisionGrounds()
        {
            oldAngle = 0.0f;
            snapPosition();
            moveDirection = Vector3.Cross(moveDirection, Vector3.down);

            setAxisAndPivot();
            setModeForward();

            if (checkForward(0.51f))
                setModeCollisionGrounds();
        }

        protected void animCollisionGrounds()
        {
            scaleProgress = Easing.Linear(Metronome.progressTime);
            if(Metronome.progressTime < 0.5f)
                transform.localScale = new Vector3(1f - scaleProgress, 1f - scaleProgress, 1f - scaleProgress);
            else
                transform.localScale = new Vector3(.5f + scaleProgress / 2, .5f + scaleProgress / 2, .5f + scaleProgress / 2);
        }
        #endregion

        #region Fall
        public void setModeFall()
        {
            move = stateFall;
            anim = fallAnim;
        }

        protected void stateFall()
        {
            snapPosition();
            startPosition = transform.localPosition;

            setAxisAndPivot();

            if (checkGround(1f))
                setModeForward();

            if (checkGround(1f) && checkForward(0.51f))
                setModeCollisionGrounds();
        }

        protected void fallAnim()
        {
            transform.position = new Vector3(startPosition.x, startPosition.y - Easing.Linear(Metronome.progressTime), startPosition.z);
        }
        #endregion

        #region Transition
        public void setModeTransition()
        {
            move = stateTransition;
            anim = transitionAnim;
        }
    
        protected void stateTransition()
        {
            oldAngle = 0.0f;
            snapPosition();
            startPosition = transform.localPosition;
            setAxisAndPivot();

            if (!checkGround(1f))
                setModeFall();
            else if (checkForward(0.51f))
                setModeCollisionGrounds();
            else
                setModeWait();
        }

        protected void transitionAnim()
        {
            if (transitionDirection < -179f) transitionDirection = 180f;
            switch ((int)transitionDirection)
            {
                case 90:
                    transform.position = new Vector3(startPosition.x + Easing.Linear(Metronome.progressTime), startPosition.y, startPosition.z);
                    break;
                case -90:
                    transform.position = new Vector3(startPosition.x - Easing.Linear(Metronome.progressTime), startPosition.y, startPosition.z);
                    break;
                case -180:
                    transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z - Easing.Linear(Metronome.progressTime));
                    break;
                case 180:
                        transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z - Easing.Linear(Metronome.progressTime));
                        break;
                default:
                    transform.position = new Vector3(startPosition.x, startPosition.y, startPosition.z + Easing.Linear(Metronome.progressTime));
                    break;
            }
        }
        #endregion

        #region Teleport
        public void setModeTeleport(Vector3 pPosition)
        {
            move = stateTeleport;
            anim = animTeleport;
            posToTeleport = pPosition;
        }

        protected void stateTeleport()
        {
            transform.localScale = new Vector3(1, 1, 1);
            snapPosition();

            setAxisAndPivot();

            oldAngle = 0.0f;
            startPosition = transform.localPosition;
            
            setModeSpawn(posToTeleport);
        }


        protected void animTeleport()
        {
            // animation scale
            scaleProgress = Easing.Bounce.InOut(Metronome.progressTime);
            transform.localScale = new Vector3(1 - scaleProgress, 1 - scaleProgress, 1 - scaleProgress);
        }
        #endregion

        #region Exit
        public void setModeExit()
        {
            move = exit;
            anim = animexit;
            SfxManager.manager.PlaySfx("SFX_Disparition");
        }

        protected void exit()
        {
            destroy();
        }

        protected void animexit()
        {
            // animation scale
            scaleProgress = Easing.Bounce.InOut(Metronome.progressTime);
            transform.localScale = new Vector3(1-scaleProgress, 1 - scaleProgress, 1 - scaleProgress);
        }
        #endregion

    #endregion

    #region Methods
    // Defini le pivot et l'axe de deplacement
    public void setAxisAndPivot()
    {
        pivot = transform.position + moveDirection * .5f - Vector3.up * .5f;
        axis = Vector3.Cross(moveDirection, Vector3.down);
    }

    // Arrondi de la position du cube
    private void snapPosition()
    {
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), Mathf.Round(transform.position.z));
    }

    // Check s'il y a un mur en dessous
    private bool checkGround(float distance)
    {
        return Physics.Raycast(transform.position, Vector3.down, distance, mask);
    }
    
    // Check s'il y a un mur en face
    private bool checkForward(float distance)
    {
        return Physics.Raycast(transform.position, moveDirection, distance, mask);
    }

    // Check s'il y a la killzone en dessous
    public bool checkKillZone()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1f, killzone);
    }
    #endregion

}
