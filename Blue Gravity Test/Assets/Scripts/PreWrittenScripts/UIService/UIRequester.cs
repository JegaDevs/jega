using UnityEngine;
using JegaCore;

namespace Jega.BlueGravity.PreWrittenCode
{
    public class UIRequester : MonoBehaviour
    {
        void Awake()
        {
            ServiceProvider.GetService<UIService>();
        }
    }

}


