using UnityEngine;
using Unity.Netcode.Components;
using System.Collections;
using System.Collections.Generic;


public class OwnerNetworkAnimator : NetworkAnimator
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }

}
