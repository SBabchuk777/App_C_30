using UnityEngine;

namespace Game.Obstacles.Enemies
{
    public class FishEnemy : Obstacle
    {
        [SerializeField] private Animator _animator = null;
        
        [Space]
        
        [SerializeField] private Rigidbody2D _rigidbody = null;

        [SerializeField] private float _moveSpeed = 1f;

        private void FixedUpdate() =>
            _rigidbody.velocity = Vector2.left * _moveSpeed;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
                _animator.SetTrigger("Attack");
        }
    }  
}
