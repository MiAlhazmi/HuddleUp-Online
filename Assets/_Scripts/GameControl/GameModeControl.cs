using UnityEngine;

namespace _Scripts.GameControl
{
    public abstract class GameModeControl : MonoBehaviour
    {
        public void BaseStart()
        {
            Run();
        }
        public virtual void Run()
        {
            
        }
    }
}