using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Example
{
    public abstract class Spell : ISpell
    {
        #region Fields

        protected UsableServices _services;
        protected IPerson[] _targets;
        protected GameObject _projectile;
        protected BaseGameType SpellColorType;
        protected SpellType SpellType;
        protected Vector3 _startPos;
        protected Vector3 _poolPos;

        protected Status[] _statuses;

        protected float _destroyDelay;
        protected float _hitRadius;
        protected float _speed;

        protected bool _isFired;
        protected bool _isHitDone;
        protected bool _isSpawnEnemy;

        #endregion


        #region Properties

        public int Id { get; set; }
        public Transform Transform { get; set; }

        #endregion


        #region ClassLifeCycles

        protected Spell(Vector3 poolPosition, SpellObject spellObject, UsableServices services)
        {
            Id = spellObject.Id;
            _speed = spellObject.Speed;
            _hitRadius = spellObject.HitRadius;
            SpellColorType = spellObject.SpellColorType;
            _destroyDelay = spellObject.DestroyAfterTime;
            _poolPos = poolPosition;
            _services = services;
            _isSpawnEnemy = spellObject.MustSpawnOnEnemyPosition;
            _statuses = new Status[spellObject.StatusObjects.Length];

            if (!_isSpawnEnemy)
            {
                Transform = spellObject.SpawnSpellPosition;
            }

            _projectile = Object.Instantiate(spellObject.Spell);
            _projectile.SetActive(false);
            _projectile.name = $"{Id}";

            int index = 0;
            foreach (StatusObject state in spellObject.StatusObjects)
            {
                _statuses[index] = Invoker.CreateStatus(state);
                index++;
            }
        }


        #endregion


        #region Methods 

        protected abstract void Effect();

        protected virtual void Starting()
        {
            _projectile.SetActive(true);

            if (_isSpawnEnemy)
            {
                var enemyPos = _targets[0].Transform.position;
                enemyPos.y -= _targets[0].Transform.lossyScale.y / 2;

                _projectile.transform.position = enemyPos;
            }
            else
            {
                _projectile.transform.position = Transform.position;
            }
        }

        protected virtual void Move()
        {
            _projectile.GetComponent<Rigidbody>().velocity = _projectile.transform.forward * _speed;
        }

        protected virtual bool HitCheck(float radius)
        {
            if (_isHitDone)
            {
                return false;
            }

            Collider[] hits = Physics.OverlapSphere(_projectile.transform.position, radius);
            for (int i = 0; i < hits.Length; i++)
            {
                if (_targets.Any(t => t.Transform == hits[i].transform))
                {
                    var target = _targets.FirstOrDefault(t => t.Transform == hits[i].transform);

                    ResetAfterTime();
                    HitEffect(target);
                    _isHitDone = true;
                }
            }
            return _isHitDone;
        }

        protected virtual void HitEffect(IPerson target)
        {
            foreach (Status status in _statuses)
            {
                target.SetStatus(status);
            }
        }

        protected virtual void ResetAfterTime()
        {
            _services.InvokeService.Invoke(Reset, _destroyDelay);
        }

        protected virtual void Reset()
        {
            _isHitDone = false;
            _isFired = false;

            _projectile.transform.position = _poolPos;
            _projectile.SetActive(false);
        }

        #endregion


        #region ISpell

        public abstract bool Use<T>(T target);

        #endregion
    }
}