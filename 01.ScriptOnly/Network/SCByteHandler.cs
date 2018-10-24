#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/StrixLibrary
 *	============================================ 	
 *	관련 링크 :
 *	
 *	설계자 : 
 *	작성자 : Strix
 *	
 *	기능 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System;

using NUnit.Framework;
using UnityEngine.TestTools;

public static class SCByteHelper
{
	static public int ConvertByte_To_Int( byte byteTarget, int iBitIndexFinish = 8, int iBitIndexStart = 0)
	{
		int iTotalValue = 0;
		BitArray arrBit = new BitArray( new byte[1] { byteTarget } );
		for (int i = iBitIndexStart; i < iBitIndexFinish; i++)
		{
			if (arrBit[i])
				iTotalValue += (int)System.Math.Pow( 2, i );
		}

		return iTotalValue;
	}

	static public BitArray ConvertByte_To_Int( byte byteTarget, out int iResult, int iBitIndexFinish = 8, int iBitIndexStart = 0 )
	{
		iResult = 0;
		BitArray arrBit = new BitArray( new byte[1] { byteTarget } );
		for (int i = iBitIndexStart; i < iBitIndexFinish; i++)
		{
			if (arrBit[i])
				iResult += (int)System.Math.Pow( 2, i );
		}

		return arrBit;
	}

    static public byte[] ConvertByteArray<Packet>(Packet pObject)
    {
        int iPacketSize = Marshal.SizeOf(pObject);
        IntPtr pBuffer = Marshal.AllocHGlobal(iPacketSize);
        Marshal.StructureToPtr(pObject, pBuffer, false);
        byte[] arrData = new byte[iPacketSize];
        Marshal.Copy(pBuffer, arrData, 0, iPacketSize);
        Marshal.FreeHGlobal(pBuffer);

        return arrData;
    }


    static public bool Convert_ToStruct<Packet>(this byte[] arrData, out Packet pObjectType)
    {
        return ConvertPacket(arrData, out pObjectType);
    }

    static public bool ConvertPacket<Packet>(byte[] arrData, out Packet pObjectType)
    {
        pObjectType = default(Packet);
        if (arrData == null)
            return false;

        int iPacketSize = Marshal.SizeOf(typeof(Packet));
        if (iPacketSize != arrData.Length)
            return false;

        IntPtr pBuffer = Marshal.AllocHGlobal(iPacketSize);
        Marshal.Copy(arrData, 0, pBuffer, iPacketSize);
        pObjectType = (Packet)Marshal.PtrToStructure(pBuffer, typeof(Packet));
        Marshal.FreeHGlobal(pBuffer);

        return true;
    }

    static public int SizeOf<Struct>()
    {
        return Marshal.SizeOf(typeof(Struct));
    }
}


#region Test
[Category("StrixLibrary")]
public class 바이트핸들러_테스트
{
    public enum ETest
    {
        ETest1
    }

    [Test]
    static public void 바이트_To_BitArray()
    {
        Assert.IsTrue(SCByteHelper.ConvertByte_To_Int(1) == 1);
        Assert.IsTrue(SCByteHelper.ConvertByte_To_Int(1, 8, 1) == 0);
        Assert.IsTrue(SCByteHelper.ConvertByte_To_Int(2, 8, 0) == 2);
        Assert.IsTrue(SCByteHelper.ConvertByte_To_Int(127, 8, 2) == (127 - 3));
    }
}
#endregion