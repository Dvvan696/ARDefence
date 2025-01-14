﻿using System.Collections;
using _Game.Scripts.Services;
using UnityEngine;
using UnityEngine.AI;
using Mirror;

namespace easyar
{
    public class Enemy : MonoBehaviour
    {
        private Transform _target = null;
        private NavMeshAgent _navMeshAgent;

        [SerializeField] private int _hp = 100;
        [SerializeField] private float _attackDamage = 10;

        private int _idMasterBarrack;

        private void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
        }

        public Transform Target
        {
            set => _target = value; //подумать над передачей таргета, когда цели нет
        }

        public int IdMasterBarrack
        {
            get => _idMasterBarrack;
            set => _idMasterBarrack = value;
        }

        public void Go()
        {
            StartCoroutine(Walk());
            _navMeshAgent.enabled = true;
            _navMeshAgent.destination = _target.position;
            
        }

        public void ApplyDamage(int damageValue)
        {
            _hp -= damageValue;
            if (_hp <= 0)
            {
                
                _navMeshAgent.enabled = false;
                StopAllCoroutines();
                _hp = 100;
                GameController.OnUnitDied(this.gameObject);
                //Debug.Log("Died");
            }
        }
        
        IEnumerator Walk()
        {
            while (true)
            {
                if (Vector3.Distance(transform.position, _target.position) < 0.05f)
                {
                    GameController.OnGetDamage(_attackDamage, _target.GetComponent<Bastille>().PlayerNumber);
                    ApplyDamage(101); //тестовое
                }

                yield return new WaitForSeconds(0.5f);
                
                // Debug.Log(_text.value);
                // transform.position = Vector3.MoveTowards(transform.position, _target.position, (float)_text.value * Time.deltaTime); //Vector3.Lerp (transform.position, _target.position, 0.1f);
                
            }
        }
    }
}