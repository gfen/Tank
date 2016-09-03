using UnityEngine;
using System.Collections;

namespace Game
{
    public class TankEnemyLife : TankLife
    {
        public Material[] GradeMaterials;

        private Renderer _bodyRenderer;

        void Start()
        {
            _bodyRenderer = transform.Find("Model/Main").GetComponent<Renderer>();
            SetMaterial();
        }

        protected sealed override void ApplyDamage(int power)
        {
            base.ApplyDamage(power);
            SetMaterial();
        }

        private void SetMaterial()
        {
            if (Life > 0 && Life <= GradeMaterials.Length)
            {
                _bodyRenderer.sharedMaterial = GradeMaterials[Life - 1];
            }
        }
    }
}
