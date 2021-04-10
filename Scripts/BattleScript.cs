﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using TMPro;
public class BattleScript : MonoBehaviourPun
{
    public Spinner spinnerScript;
    public GameObject uI_3D_GameObject;
    private Rigidbody rb;

    public GameObject deathPanelUIPrefab;
    private GameObject deathPanelUIGameObject;

    private float startSpinSpeed;
    private float currentSpinSpeed;
    public Image spinSpeedBar_Image;
    public TextMeshProUGUI spinSpeedRatio_Text;

    public float common_Damage_Cofficient = 0.04f;
    public bool isAttacker;
    public bool isDefender;
    private bool isDead = false;
    [Header("Player Type Damage Coefficients")]
    public float doDamage_Coefficient_Attacker = 10f;
    public float getDamaged_Coefficient_Attacker = 1.2f;

    public float doDamage_Coefficient_Defender = 0.75f;
    public float getDamaged_Coefficient_Defender = 0.2f;
    // Start is called before the first frame update

    private void Awake()
    {
        startSpinSpeed = spinnerScript.spinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;
        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
    }

    private void CheckPlayerType()
    {
        if(gameObject.name.Contains("Attacker"))
        {
            isAttacker = true;
            isDefender = false;
        }
        else if(gameObject.name.Contains("Defender"))
        {
            isDefender = true;
            isAttacker = false;
            spinnerScript.spinSpeed = 4400;
            startSpinSpeed = spinnerScript.spinSpeed;
            currentSpinSpeed = spinnerScript.spinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            if (photonView.IsMine)
            {
                Vector3 effectPosition = (gameObject.transform.position + collision.transform.position) / 2 + new Vector3(0, 0.05f, 0);

                //Instantiate Collision Effect ParticleSystem
                GameObject collisionEffectGameobject = GetPooledObject();
                if (collisionEffectGameobject != null)
                {
                    collisionEffectGameobject.transform.position = effectPosition;
                    collisionEffectGameobject.SetActive(true);
                    collisionEffectGameobject.GetComponentInChildren<ParticleSystem>().Play();

                    //De-activate Collision Effect Particle System after some seconds.
                    StartCoroutine(DeactivateAfterSeconds(collisionEffectGameobject, 0.5f));

                }
            }
            float mySpeed = gameObject.GetComponent<Rigidbody>().velocity.magnitude;
            float otherPlayerSpeed = collision.collider.gameObject.GetComponent<Rigidbody>().velocity.magnitude;

            Debug.Log("My Speed: " + mySpeed + "Other Player Speed: " + otherPlayerSpeed);
            if(mySpeed > otherPlayerSpeed)
            {
                float default_Damage_Amount = gameObject.GetComponent<Rigidbody>().velocity.magnitude * 3600f * common_Damage_Cofficient;
                Debug.Log("You damage other");
                if (isAttacker)
                {
                    default_Damage_Amount = default_Damage_Amount * doDamage_Coefficient_Attacker;

                }
                else if (isDefender)
                {
                    default_Damage_Amount = default_Damage_Amount * doDamage_Coefficient_Defender;
                }

                if (collision.collider.gameObject.GetComponent<PhotonView>().IsMine)
                {
                    
                    collision.collider.gameObject.GetComponent<PhotonView>().RPC("DoDamage", RpcTarget.AllBuffered, default_Damage_Amount);

                }
            }
     
        }
    }

    [PunRPC]
    public void DoDamage(float _damageAmount)
    {   
        if(!isDead)
        {
            if (isAttacker)
            {
                _damageAmount *= getDamaged_Coefficient_Attacker;

                if(_damageAmount>500)
                {
                    Debug.Log("TRUEEE");
                    _damageAmount = 400f;
                }
            }
            else if (isDefender)
            {
                _damageAmount *= getDamaged_Coefficient_Defender;
            }
            spinnerScript.spinSpeed -= _damageAmount;
            currentSpinSpeed = spinnerScript.spinSpeed;
            spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
            spinSpeedRatio_Text.text = currentSpinSpeed.ToString("F0") + "/" + startSpinSpeed;
            Debug.Log(spinSpeedRatio_Text.text + " " + startSpinSpeed);
            if (currentSpinSpeed < 100)
            {
                //Die
                Die();
            }
        }
       
    }
    void Die()
    {
        isDead = true;
        GetComponent<MovementController>().enabled = false;
        rb.freezeRotation = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        spinnerScript.spinSpeed = 0f;
        uI_3D_GameObject.SetActive(false);

        if(photonView.IsMine)
        {
            StartCoroutine(ReSpawn());
        }
    }
    IEnumerator ReSpawn()
    {
        GameObject canvasGameObject = GameObject.Find("Canvas");
        if (deathPanelUIGameObject == null)
        {
            deathPanelUIGameObject = Instantiate(deathPanelUIPrefab, canvasGameObject.transform);
        }
        else
        {
            deathPanelUIGameObject.SetActive(true);
        }
        Text reSpawnTimeText = deathPanelUIGameObject.transform.Find("RespawnTimeText").GetComponent<Text>();

        float respawnTime = 8.0f;

        reSpawnTimeText.text = respawnTime.ToString(".00");
        while(respawnTime>0.0f)
        {
            yield return new WaitForSeconds(1.0f);
            respawnTime -= 1.0f;
            reSpawnTimeText.text = respawnTime.ToString(".00");

            GetComponent<MovementController>().enabled = false;
        }

        deathPanelUIGameObject.SetActive(false);
        GetComponent<MovementController>().enabled = true;
        photonView.RPC("ReBorn", RpcTarget.AllBuffered);

    }
    [PunRPC]
    public void ReBorn()
    {
        spinnerScript.spinSpeed = startSpinSpeed;
        currentSpinSpeed = spinnerScript.spinSpeed;

        spinSpeedBar_Image.fillAmount = currentSpinSpeed / startSpinSpeed;
        spinSpeedRatio_Text.text = currentSpinSpeed + "/" + startSpinSpeed;
        rb.freezeRotation = true;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        uI_3D_GameObject.SetActive(true);
        isDead = false; 
    }
    void Start()
    {
        CheckPlayerType();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
