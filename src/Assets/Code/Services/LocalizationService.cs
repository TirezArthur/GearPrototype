using System;
using System.IO.Hashing;
using System.Text;
using UnityEngine;

public sealed class LocalizationService : Service
{
    public static ulong HasKey(string key)
    {
        Span<byte> bytes = stackalloc byte[Encoding.UTF8.GetByteCount(key)];
        Encoding.UTF8.GetBytes(key, bytes);
        return XxHash64.HashToUInt64(bytes);
    }

    protected override void Awake()
    {
        base.Awake();
        Locale english = Resources.Load<Locale>("Localization/English");
    }
}
