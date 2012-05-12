// Type: System.Security.Cryptography.HashAlgorithm
// Assembly: mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll

using System;
using System.IO;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Security.Cryptography
{
[ComVisible(true)]
public abstract class HashAlgorithm : ICryptoTransform, IDisposable
{
    protected int HashSizeValue;
    protected internal byte[] HashValue;
    protected int State;

    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    protected HashAlgorithm();

    public virtual int HashSize
    {
        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        get;
    }

    public virtual byte[] Hash
    {
        get;
    }

    #region ICryptoTransform Members

    [SecuritySafeCritical]
    public int TransformBlock(byte[] inputBuffer, int inputOffset, int inputCount, byte[] outputBuffer, int outputOffset);

    [SecuritySafeCritical]
    public byte[] TransformFinalBlock(byte[] inputBuffer, int inputOffset, int inputCount);

    [SecuritySafeCritical]
    public void Dispose();

    public virtual int InputBlockSize
    {
        get;
    }
    public virtual int OutputBlockSize
    {
        get;
    }
    public virtual bool CanTransformMultipleBlocks
    {
        get;
    }
    public virtual bool CanReuseTransform
    {
        get;
    }

    #endregion

    [SecuritySafeCritical]
    [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
    public static HashAlgorithm Create();

    [SecuritySafeCritical]
    public static HashAlgorithm Create(string hashName);

    public byte[] ComputeHash(Stream inputStream);
    public byte[] ComputeHash(byte[] buffer);
    public byte[] ComputeHash(byte[] buffer, int offset, int count);

    public void Clear();
    protected virtual void Dispose(bool disposing);
    public abstract void Initialize();
    protected abstract void HashCore(byte[] array, int ibStart, int cbSize);
    protected abstract byte[] HashFinal();
}
}
