using UnityEngine;

namespace BlockBreaker3D.Models.InGame.Component
{
    public class HaveLifeComp : Comp
    {
        [SerializeField] private int _life;
        [SerializeField] private int _defaultLife;
        [SerializeField] private bool _isAlive;

        public int Life => _life;
        public int DefaultLife => _defaultLife;
        public bool IsAlive => _isAlive;
        public HaveLifeComp(int life) : base(false)
        {
            _defaultLife = _life = life;
            _isAlive = true;
        }

        public void ResetLife()
        {
            _life = DefaultLife;
            _isAlive = true;
        }

        public void IncreaseLife(int amount)
        {
            _life += amount;
        }
        public void DecreaseLife(int amount)
        {
            _life -= amount;
            if (Life <= 0)
            {
                _life = 0;
                _isAlive = false;
            }
        }

        public void SetDefaultLife(int life)
        {
            _defaultLife = life;
        }

        public void SetAriveState(bool state)
        {
            _isAlive = state;
        }
    }
}