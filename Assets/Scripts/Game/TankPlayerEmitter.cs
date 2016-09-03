using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Networking;

namespace Game
{
    public class TankPlayerEmitter : TankEmitter
    {
        protected override void FireUpdate()
        {
            if (!hasAuthority) return;

            if (CrossPlatformInputManager.GetAxis("Fire") > 0.1f)
            {
                StartFire();
                CmdFire();
            }
        }

        [Command]
        private void CmdFire()
        {
            ServerFire();
        }
    }
}
