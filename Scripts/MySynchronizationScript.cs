using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class MySynchronizationScript : MonoBehaviour, IPunObservable
{
    Rigidbody rb;
    PhotonView photonView;

    Vector3 networkedPosition;
    Quaternion networkedRotation;


    public bool synchronizeVelocity = true;
    public bool synchronizeAngularVelocity = true;
    public bool isTeleportEnabled = true;
    public float teleportIfDistanceGreaterThan = 1.0f;
    private float distance;
    private float angle;

    private GameObject battleArenaGameObject;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        
        photonView = GetComponent<PhotonView>();

        networkedPosition = new Vector3();
        networkedRotation = new Quaternion();
        battleArenaGameObject = GameObject.Find("BattleArena");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void FixedUpdate()
    {
        //Debug.Log(photonView);
       if(!photonView.IsMine)
        {
           rb.position = Vector3.MoveTowards(rb.position, networkedPosition, distance*(1.0f/PhotonNetwork.SerializationRate));
           rb.rotation = Quaternion.RotateTowards(rb.rotation, networkedRotation, angle*(1.0f / PhotonNetwork.SerializationRate)); // 100 makes rotation change more visible
        }
 
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            //photon view belongs to this user
            //send my position 
            stream.SendNext(rb.position-battleArenaGameObject.transform.position);
            stream.SendNext(rb.rotation);

            if(synchronizeVelocity == true)
            {
                stream.SendNext(rb.velocity);

            }
            if(synchronizeAngularVelocity == true)
            {
                stream.SendNext(rb.angularVelocity);
            }
        }
        else
        {
            networkedPosition = (Vector3)stream.ReceiveNext()+ battleArenaGameObject.transform.position;
            networkedRotation = (Quaternion)stream.ReceiveNext();

            if(isTeleportEnabled)
            {
                if(Vector3.Distance(rb.position, networkedPosition)>teleportIfDistanceGreaterThan)
                {
                    rb.position = networkedPosition;
                }
            }

            if(synchronizeAngularVelocity || synchronizeVelocity)
            {
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));

                if(synchronizeVelocity)
                {
                    rb.velocity = (Vector3)stream.ReceiveNext();
                    networkedPosition += rb.velocity * lag;
                    distance = Vector3.Distance(rb.position, networkedPosition);
                }
                if (synchronizeAngularVelocity)
                {
                    rb.angularVelocity = (Vector3)stream.ReceiveNext();
                    networkedRotation = Quaternion.Euler(rb.angularVelocity * lag)*networkedRotation;
                    angle = Quaternion.Angle(rb.rotation, networkedRotation);
                }
            }
        }
    }
}
