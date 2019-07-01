#region Header
/* ============================================ 
 *			    Strix Unity Library
 *		https://github.com/KorStrix/UnityLibrary
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

public static class ByteExtension
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