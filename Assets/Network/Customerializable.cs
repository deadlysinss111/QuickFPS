using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomSerializable : INetworkSerializable
{
    public RaycastHit _hit;

    public CustomSerializable(RaycastHit hit)
    {
        _hit = hit;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        //serializer.SerializeValue(ref _hit);
    }
}
