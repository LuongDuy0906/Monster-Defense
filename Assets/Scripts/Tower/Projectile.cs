using UnityEngine;

public class Projectile : MonoBehaviour
{
   private TowerData _data;
   private Vector2 _shootDirection;
   private float _projectileDuration;

    void Update()
    {
        if(_projectileDuration <= 0)
        {
            gameObject.SetActive(false);
        } else
        {
            _projectileDuration += Time.deltaTime;
            transform.position += new Vector3(_shootDirection.x, _shootDirection.y) * _data.projectileSpeed * Time.deltaTime; 
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            enemy.TakeDamage(_data.damage);
            gameObject.SetActive(false);
        }
    }

    public void Shoot(TowerData data, Vector2 shootDirection)
    {
        _data = data;
        _shootDirection = shootDirection;
        _projectileDuration = data.projectileDuration;
    }
}
