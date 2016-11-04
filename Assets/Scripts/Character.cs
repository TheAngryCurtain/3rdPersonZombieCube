using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
    public GameObject bulletholePrefab;
    public GameObject bloodSplatter;
    public ParticleSystem shotParticles;
    public GameObject flashLight;

    private Transform cameraTransform;
    private Transform playerTransform;
    private float fireDelay = 0.5f;
    private float lastFireTime = 0f;
    private float moveSpeed = 5f;
    private float rotateSpeed = 100f;
    private float maxDistance = 100f;
    private float jumpForce = 300f;
    private float explosiveForce = 5f;
    private GameObject target = null;
    private RaycastHit hit;
    private Ray ray;
    private Health health;
    private int maxHealth = 25;
    private int healthPickupAmount = 25;
    private int currentAmmo;
    private int maxAmmo = 30;
    private int currentBatteryLife;
    private int maxBatteryLife = 100;
    private float batteryDrainDelay = 0.5f;
    private float batteryChargeDelay = 0.8f;
    private int headShotHits = 3;

    void Awake()
    {
        cameraTransform = Camera.main.transform;
        playerTransform = transform;

        health = new Health(maxHealth);
        currentAmmo = maxAmmo;
        currentBatteryLife = maxBatteryLife;

        ToggleLight();

        // update ui
        UIManager.Instance.UpdatePlayerAmmo(currentAmmo, maxAmmo);
        UIManager.Instance.UpdatePlayerHealth(health.CurrentHealth, health.MaxHealth);
    }

    public void Move(float h, float v)
    {
        playerTransform.Translate(Vector3.forward * v * moveSpeed * Time.deltaTime);
        playerTransform.Translate(Vector3.right * h * moveSpeed * Time.deltaTime);
    }

    public void Rotate(float h, float v, int invertModifier)
    {
        playerTransform.Rotate(Vector3.up, rotateSpeed * h * Time.deltaTime);
        cameraTransform.RotateAround(playerTransform.position, playerTransform.right, rotateSpeed * GameManager.currentSensitivity * (v * invertModifier) * Time.deltaTime);
    }

    void Update()
    {
        ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            target = hit.collider.gameObject;
        }
        else
        {
            target = null;
        }
    }

    public void MultipleHits(int numHits)
    {
        for (int i = 0; i < numHits; ++i)
        {
            TakeHit();
        }
    }

    public void TakeHit()
    {
        bool isDead = health.TakeHit();
        UpdateHealthUI();
        if (isDead)
        {
            GetComponent<Rigidbody>().freezeRotation = false;
            EnableControl(false);

            GameManager.Instance.NotifyPlayerDead();
        }
    }

    private void HandleZombieCollision(Collision other)
    {
        if (!other.gameObject.GetComponent<Zombie>().IsDead)
        {
            GetComponent<Rigidbody>().AddExplosionForce(5f, other.contacts[0].point, 0f, 0f, ForceMode.Impulse);
            TakeHit();
        }
    }

    void OnCollisionStay(Collision other)
    {
        if (other.collider.gameObject.tag == "Zombie")
        {
            HandleZombieCollision(other);
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.gameObject.tag == "Zombie")
        {
            HandleZombieCollision(other);
        }
        else if (other.collider.gameObject.tag == "Ammo")
        {
            currentAmmo = maxAmmo;
            UIManager.Instance.UpdatePlayerAmmo(currentAmmo, maxAmmo);
            Destroy(other.gameObject);
        }
        else if (other.collider.gameObject.tag == "Health")
        {
            health.AddHealth(healthPickupAmount);
            UIManager.Instance.UpdatePlayerHealth(health.CurrentHealth, health.MaxHealth);
            Destroy(other.gameObject);
        }
    }

    private void PlayerDead()
    {
        GameManager.Instance.Restart();
    }

    public void Jump()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, 1.25f))
        {
            transform.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce);
        }
    }

    public void Shoot()
    {
        if (Time.time > lastFireTime + fireDelay && currentAmmo > 0)
        {
            lastFireTime = Time.time;
            shotParticles.Play();

            if (target != null)
            {
                Quaternion hitNormal = Quaternion.FromToRotation(Vector3.up, hit.normal);
                GameObject hole = Instantiate(bulletholePrefab, hit.point + (hit.normal * 0.01f), hitNormal) as GameObject;
                hole.transform.SetParent(hit.collider.gameObject.transform);

                if (target.tag == "ShootableObject")
                {
                    target.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce, hit.point, 0f, 0f, ForceMode.Impulse);
                }
                else if (target.tag == "ExplodableObject")
                {
                    target.GetComponent<ExplosiveObject>().Explode();
                }
                else if (target.tag == "Spawner")
                {
                    target.GetComponent<Spawner>().Hit();
                }
                else if (target.tag == "Zombie")
                {
                    hole.transform.GetChild(0).GetComponent<MeshRenderer>().materials[0].color = Color.red;
                    Instantiate(bloodSplatter, hit.point, hitNormal);

                    if (hit.point.y > 0.75f)
                    {
                        // head shot
                        target.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce * 3f, hit.point, 0f, 0f, ForceMode.Impulse);
                        target.GetComponent<Zombie>().MultipleHits(headShotHits);
                    }
                    else
                    {
                        target.GetComponent<Rigidbody>().AddExplosionForce(explosiveForce, hit.point, 0f, 0f, ForceMode.Impulse);
                        target.GetComponent<Zombie>().Hit();
                    }
                }

                currentAmmo -= 1;
                GameManager.Instance.Data.shotsFired += 1;
                UIManager.Instance.UpdatePlayerAmmo(currentAmmo, maxAmmo);
            }
        }
    }

    public void ToggleLight()
    {
        flashLight.SetActive(!flashLight.activeSelf);

        if (flashLight.activeInHierarchy)
        {
            CancelInvoke("ChargeBattery");
            InvokeRepeating("DrainBattery", 0f, batteryDrainDelay);
        }
        else
        {
            CancelInvoke("DrainBattery");
            InvokeRepeating("ChargeBattery", 0f, batteryChargeDelay);
        }
    }

    private void DrainBattery()
    {
        if (currentBatteryLife > 0)
        {
            currentBatteryLife -= 1;
            UIManager.Instance.UpdateFlashLightBattery(currentBatteryLife, maxBatteryLife);
        }
        else
        {
            flashLight.SetActive(false);
        }
    }

    private void ChargeBattery()
    {
        if (currentBatteryLife < maxBatteryLife)
        {
            currentBatteryLife += 1;
            UIManager.Instance.UpdateFlashLightBattery(currentBatteryLife, maxBatteryLife);
        }
    }

    private void EnableControl(bool enable)
    {
        GetComponent<PlayerController>().enabled = enable;
    }

    private void UpdateHealthUI()
    {
        UIManager.Instance.UpdatePlayerHealth(health.CurrentHealth, health.MaxHealth);
    }
}
