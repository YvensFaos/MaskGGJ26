using UnityEngine;
using UUtils;

namespace Core
{
    public class GameManager : StrongSingleton<GameManager>
    {
        [SerializeField]
        private DebugUtils.DebugType enabledDebugTypes = DebugUtils.DebugType.Regular | DebugUtils.DebugType.System | DebugUtils.DebugType.Warning | DebugUtils.DebugType.Error;

        private void OnValidate()
        {
            DebugUtils.enabledDebugTypes = enabledDebugTypes;
        }
    }
}