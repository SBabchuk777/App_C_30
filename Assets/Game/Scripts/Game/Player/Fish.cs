using System;
using DG.Tweening;
using Game.Road;
using Prototype.AudioCore;
using UnityEngine;

namespace Game.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Fish : MonoBehaviour
    {
        public event Action OnDead = null;

        [SerializeField] private float _velocity = 10f;

        private Animator _animator;
        private Rigidbody2D _rigidbody;

        private Tutorial _tutorial;

        public bool IsDead { get; private set; }

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _rigidbody = GetComponent<Rigidbody2D>();

            _tutorial = FindObjectOfType<Tutorial>();

            _rigidbody.bodyType = RigidbodyType2D.Kinematic;

            _tutorial.OnComplete += () =>
            {
                _rigidbody.bodyType = RigidbodyType2D.Dynamic;

                MoveUp();
            };
        }

        private void Update()
        {
            if (IsDead)
                return;

            if (_tutorial.IsCompleted)
            {
                if (Input.GetMouseButton(0))
                    MoveUp();
            }
            else
            {
                _rigidbody.velocity = Vector2.right * _velocity;
            }
        }

        private void MoveUp()
        {
            if (!AudioController.IsSoundPlaying("fish_up"))
                AudioController.PlaySound("fish_up");
            
            _rigidbody.velocity = (Vector2.up + Vector2.right) * _velocity;
        }

        private void Kill()
        {
            IsDead = true;

            AudioController.StopSound("fish_up");
            
            AudioController.PlaySound("result_panel");

            _rigidbody.velocity = Vector2.zero;
            _rigidbody.mass = 10f;

            _animator.SetTrigger("Damage");

            OnDead?.Invoke(); 
        }
        
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (IsDead)
                return;

            if (collision.gameObject.CompareTag("DeadCollider"))
                Kill();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (IsDead)
                return;

            if (collision.CompareTag("DeadCollider"))
                Kill();
            
            if (collision.CompareTag("Bonus"))
            {
                AudioController.PlaySound("collect");

                Bonus bonus = collision.GetComponent<Bonus>();

                Destroy(collision);

                bonus.transform.DOScale(0f, 0.25f);

                Wallet.AddMoney(bonus.BonusPoints);
            }
        }
    }
}