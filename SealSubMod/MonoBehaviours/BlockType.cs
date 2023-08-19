using SealSubMod.Data;
using SealSubMod.MonoBehaviours.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod.MonoBehaviours;

internal class BlockType : ApplyForceTrigger
{
    internal override Vector3 GetPushDirection(Rigidbody rigidbody)
    {
        return transform.forward;
    }
}
