using System.Collections.Generic;


namespace Example
{
    public class SpellsController : IExecuteController, ICleanupController
    {
        #region Fields

        private readonly GameContext _context;
        private UsableServices _services;
        private List<ISpell> _activeSpells = new List<ISpell>(); 
        private List<int> _remoteSpells = new List<int>(); 

        #endregion


        #region ClassLifeCycles

        public SpellsController(GameContext context, UsableServices services)
        {
            _context = context;
            _services = services;
        }

        #endregion


        #region IExecuteController

        public void Execute()
        {
            var id = 0;

            if (_context.ActiveSpell != null)
            {
                _activeSpells.Add(_context.ActiveSpell);
            }

            foreach (ISpell spell in _activeSpells)
            {
                if (spell.Use(_context.Enemies))
                {
                    _remoteSpells.Add(id);
                }

                id++;
            }
        }

        #endregion


        #region ICleanupController

        public void Cleanup() 
        {
            _context.ActiveSpell = null;

            foreach (var id in _remoteSpells)
            {
                _context.Spells.PutObject(_activeSpells[id]);
                _activeSpells.RemoveAt(id);
            }

            _remoteSpells.Clear();
        }
        
        #endregion
    }
}