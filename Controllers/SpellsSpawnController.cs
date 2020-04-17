using System.Linq;
using UnityEngine;

namespace Example
{
    public class SpellsSpawnController : IInitializeController, IExecuteController, ICleanupController
    {
        #region PrivateData

        private readonly GameContext _context;
        private UsableServices _services;
        private PoolObject<ISpell> _pool;
        private Vector3 _poolPosition;

        #endregion


        #region ClassLifeCycle

        public SpellsSpawnController(GameContext context, UsableServices services)
        {
            _context = context;
            _services = services;
        }

        #endregion

        
        #region IInitializeController

        public void Initialize()
        {
            var poolObjectSpellPosition = _context.SpellData.SpellSettings.PoolObjectSpellPosition.position;
            _pool = new PoolObject<ISpell>();
            _poolPosition = poolObjectSpellPosition;
            _context.Spells = _pool;

            var charIndex = 0;
            while (charIndex < _context.Characters.Count)
            {
                foreach (var spell in _context.SpellData.SpellObjects)
                {
                    if (spell.SpellType == _context.Characters.ElementAt(charIndex).Spell)
                    {
                        var size = 0;
                        while (size < spell.PoolSize)
                        {
                            _pool.PutObject(Invoker.CreateSpell(_poolPosition, spell, _services)());
                            size++;
                        }
                    }
                }
                charIndex++;
            }
        }

        #endregion


        #region IExecuteController

        public void Execute()
        {
            if (_context.ActiveSpellObject != null)
            {
                _context.ActiveSpell = _pool.GetObject(_context.ActiveSpellObject.Id, Invoker.CreateSpell(_poolPosition, _context.ActiveSpellObject, _services));
            }
        }

        #endregion


        #region ICleanupController

        public void Cleanup()
        {
            if (_context.ActiveSpell != null)
            {
                _context.ActiveSpellObject = null;
                _context.ActiveSpell = null;
            }
        }

        #endregion
    }
}