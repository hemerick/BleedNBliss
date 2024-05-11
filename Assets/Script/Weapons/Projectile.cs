using UnityEngine;

public interface IWeaponDamage
{
    void ProjectileInflictDamage(float damage);
}

public enum ProjectileSource 
{
    Player,
    Enemy
}

public abstract class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float lifetime = 1f; //MAKE THIS == TO RANGE VALUE
    public ProjectileSource source;
    protected float currentLifetime = 1f;
    protected float damage = 1;

    protected Vector3 movement;

    public void Reset()
    {
        CustomReset();
        movement = transform.rotation * Vector3.right;
    }

    protected abstract void CustomReset();

    void Update()
    {
        currentLifetime -= Time.deltaTime;
        if (currentLifetime < 0)
        {
            gameObject.SetActive(false);
        }
        Movement();
    }
    protected abstract void Movement();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(source == ProjectileSource.Player && collision.CompareTag("Enemy"))
        {
            var hittedTarget = collision.GetComponent<IWeaponDamage>();
            hittedTarget?.ProjectileInflictDamage(damage);
            //gameObject.SetActive(false)
        }
        if(source == ProjectileSource.Player && collision.CompareTag("SoftSpot"))
        {
            var hittedTarget = collision.GetComponentInParent<IWeaponDamage>();
            hittedTarget?.ProjectileInflictDamage(damage*2);
            //Debug.Log("COUP CRITIQUE : " + damage * 2 + " DMG!");
            gameObject.SetActive(false);
        }
        else if(source == ProjectileSource.Enemy && collision.CompareTag("Player"))
        {
            var player = collision.GetComponent<IWeaponDamage>();
            player?.ProjectileInflictDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
