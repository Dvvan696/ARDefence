﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using _Game.Scripts.Services;
using easyar;

namespace _Game.Scripts.Enemies
{
    public class StandartBarrack : MonoBehaviour, IBaseBarrack
    {
        [SerializeField] private int _idBarrack = 0;
        [SerializeField] private OwnerController _ownerController;
        [SerializeField] private List<GameObject> _solders = new List<GameObject>();
        
        [SerializeField] private float _timeoutSpawn = 1f;
        
        [SerializeField] private GameObject _target;

        [SerializeField] private Transform _startPosition;
        [SerializeField] private Transform _storeUnitPosition; // vector3

        public void Start()
        {
            //EventCrossroad.UnitDiedEvent += AddSolder;
            GameController.UnitDiedEvent += ReturnUnit;//подпись на события
            
            GameController.NumberOfBarracks += 1;
            _idBarrack = GameController.NumberOfBarracks;
            
            //тут дописать
                //надо что бы барраки били чужих а не своих, а то сейчас правильно только у хоста, клиент сосёт жопу, опять
            //и типа при постройке в позитионтрекер присваивался бы номер игрока, а пока ебануть тестовое в бастиллии 
            
            foreach (var solder in _solders)
            {
                solder.GetComponent<Enemy>().IdMasterBarrack = _idBarrack;
            }

            StartCoroutine(WaitChangeOwner());
        }
        
        private IEnumerator WaitChangeOwner()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.01f);
                if (GetComponent<OwnerController>().GetOwner() == -1) continue;
                StartAttack();
                yield break;
            }
        }

        private void StartAttack()
        {
            print("OwN " + _ownerController.GetOwner() + " - PlN " + GameController.Player.PlayerNumber + " ImN " + GameController.Imposter.PlayerNumber);
            _target = _ownerController.GetOwner() == GameController.Player.PlayerNumber ? GameController.Imposter.gameObject : GameController.Player.gameObject;
            print(_target.tag);
            StartCoroutine(Spawn());
        }

        public void AddSolder(GameObject solder)
        {
            _solders.Add(solder);
            //Debug.Log("Solder added");
        }

        public void ExitSolder()
        {
            _solders.RemoveAt(0);
            //Debug.Log("Solder came out");
        }

        private void ReturnUnit(GameObject unit)
        {
            if (unit.GetComponent<Enemy>().IdMasterBarrack == _idBarrack)
            {
                unit.transform.position = _storeUnitPosition.position;
                AddSolder(unit);
            }
        }
        IEnumerator Spawn()
        {
            while (true)
            {
                //Debug.Log("Start");
                yield return new WaitForSeconds(_timeoutSpawn);
                if (_target.activeSelf && _solders.Any())
                {
                    _solders[0].transform.position = _startPosition.position;
                    _solders[0].GetComponent<Enemy>().Target = _target.transform;
                    _solders[0].GetComponent<Enemy>().Go();
                    ExitSolder();
                }
            }
        }
    }
}