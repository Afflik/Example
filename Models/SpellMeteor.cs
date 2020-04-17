using System.Linq;
using UnityEngine;
using System.Collections.Generic;


namespace Example
{
    public sealed class SpellMeteor : Spell
    {
        #region PrivateData

        private Flinder[] _flinders;
        private Transform[] _savedFlinders;

        #endregion


        #region ClassLifeCycle

        public SpellMeteor(Vector3 poolPosition, SpellObject spellObject, UsableServices services) : base(poolPosition, spellObject, services)
        {
            _savedFlinders = _projectile.GetComponentsInChildren<Transform>();

            var statuses = new List<Status>();
            foreach (var statusObject in spellObject.StatusObjects)
            {
                statuses.Add(Invoker.CreateStatus(statusObject));
            }
            _statuses = statuses.ToArray();
        }

        #endregion


        #region Methods

        protected override void Effect()
        {
            Starting();
            SaveFlinders();
            _projectile.transform.LookAt(_targets.FirstOrDefault().Transform);
            Move();
        }

        private void SaveFlinders()
        {
            _flinders = new Flinder[_savedFlinders.Length];

            for (int i = 0; i < _flinders.Length; i++)
            {
                _flinders[i].position = _savedFlinders[i].localPosition;
                _flinders[i].rotation = _savedFlinders[i].localRotation;
                _flinders[i].scale = _savedFlinders[i].localScale;
            }
        }

        private void LoadFlinders()
        {
            for (int i = 0; i < _savedFlinders.Length; i++)
            {
                _savedFlinders[i].localPosition = _flinders[i].position;
                _savedFlinders[i].localRotation = _flinders[i].rotation;
                _savedFlinders[i].localScale = _flinders[i].scale;
            }
        }

        private void RigAndMeshFlinders(bool toggle)
        {
            foreach (Rigidbody bit in _projectile.GetComponentsInChildren<Rigidbody>())
            {
                if (bit.transform.parent != null) bit.isKinematic = toggle;
            }
            foreach (MeshCollider col in _projectile.GetComponentsInChildren<MeshCollider>())
            {
                if (col.transform.parent != null) col.isTrigger = toggle;
            }
        }

        #endregion


        #region ISpell

        public override bool Use<T>(T target)
        {
            if (!_isFired) 
            {
                _isFired = true;
                if (target is IEnumerable<IPerson> targets)
                {
                    _targets = targets.ToArray();
                    Effect();
                }
            }

            return HitCheck(_hitRadius);
        }

        protected override bool HitCheck(float radius)
        {
            if(base.HitCheck(_hitRadius))
            {
                RigAndMeshFlinders(false);
                return true;
            }
            return false;
        }

        protected override void Reset()
        {
            RigAndMeshFlinders(true);
            LoadFlinders();
            base.Reset();
        }

        #endregion
    }
}