using UnityEngine;

namespace BattleCity.Stats
{
    public class EnemyStats : EntityStats
    {
        [SerializeField]
        private int _bounty;

        private bool _isBonus;

        public bool IsBonus
        {
            get => _isBonus;
            set
            {
                _isBonus = value;
            }
        }

        public int Bounty => _bounty;
    }
}
