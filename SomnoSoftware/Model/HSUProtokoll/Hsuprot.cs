using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SomnoSoftware
{
    class Hsuprot
    {
        //-----------------------------------------------------------------------------
        //     File:          qhsuprot.h
        //                    22.09.14
        //     Description:   HS-Ulm Serial Protocol
        //                   
        //
        //	Version:
        //            _.__	see qhsuprot.cpp !!!
        //-----------------------------------------------------------------------------
        //#ifndef QHSUPROT_H
        //const int QHSUPROT_H

        //##############################################################################

        const int MAXPCKSIZE = 255;

        // Transmission Flags
        const byte STARTFLAG = 170;     // signs begin of a new transmission
        const byte STOPFLAG = 171;      // signs end of transmission
        const byte ESCFLAG = 0xAF;       // signs next byte is coded with ESCAPEMASK

        const byte ESCMASK = 0x80;       // TX -> byte &= ~ESCMASK / RX -> byte |= ESCMASK

        const byte IDMESSAGE = 0x80;     // Confirmation messages  == _Command |= 0x80
        const byte IDSIZE = 0x20;        // PACKAGESIZE instead of ESCAPEFLAGS and CHECKSUM
        const byte IDERROR = 0xFF;       // Sensor Error-ID

        // PackageFlags (sign Package-State)
        const byte PCKCMPL = 0x01;       // Complete (previous packet complete)
        const byte PCKID = 0x02;         // IDentity byte received
        const byte PCKSIZE = 0x04;       // PackageSize byte received
        const byte PCKESC = 0x08;        // previous byte was an escapeflag
        const byte PERRSTOP = 0x10;      // communication record errors
        const byte PERRSTART = 0x20;
        const byte PERRCHKSUM = 0x40;
        const byte PERRCMD = 0x80;

        public class serialPck // structSerialPck
        {
            public byte ID;
            public ushort Size;
            public ushort SizeCnt;
            public byte[] Bytes = new byte[MAXPCKSIZE];
            public byte ChkSum;
            public byte Flags;
        }

        private ushort extPckSize;
        private serialPck rxPck_ = new serialPck();       // temp. Package to handle incomming bytes
        public serialPck inPck_ = new serialPck();        // the last complete rxPck_
        private serialPck outPck_ = new serialPck();


        public void Reset()
        {
            rxPck_.Flags = PCKCMPL;
            rxPck_.ID = 0;
            rxPck_.Size = 0;
        }

        public bool MakePck(ref serialPck outPackage)
        {
            outPck_.Bytes[0] = STARTFLAG;               // STARTFLAG
            if ((outPackage.ID == STARTFLAG)
                    || (outPackage.ID == STOPFLAG)         // ID != FLAG !!!
                    || (outPackage.ID == ESCFLAG))
                return false;

            outPck_.ID = outPackage.ID;
            outPck_.Bytes[1] = outPackage.ID;
            outPck_.Size = 2;
            outPck_.ChkSum = outPackage.ID;

            for (int i = 0; i < outPackage.Size - 1; i++)  // DATA
            {
                if ((outPackage.Bytes[i] == STARTFLAG)
                        || (outPackage.Bytes[i] == STOPFLAG)
                        || (outPackage.Bytes[i] == ESCFLAG))
                {
                    outPck_.Bytes[outPck_.Size++] = ESCFLAG;
                    if (!(outPck_.Size != 0))
                        return false;                   // overflow occured
                    outPackage.Bytes[i] = (byte)(~ESCMASK & outPackage.Bytes[i]);
                }
                outPck_.Bytes[outPck_.Size++] = outPackage.Bytes[i];
                if (!(outPck_.Size != 0))
                    return false;                       // overflow occured
                outPck_.ChkSum += outPackage.Bytes[i];  // calc. chkSum;
            }

            if ((outPck_.ChkSum == STARTFLAG)
                    || (outPck_.ChkSum == STOPFLAG)
                    || (outPck_.ChkSum == ESCFLAG))
            {
                outPck_.Bytes[outPck_.Size++] = ESCFLAG;
                outPck_.ChkSum = (byte)(~ESCMASK & outPck_.ChkSum);
            }
            outPck_.Bytes[outPck_.Size++] = outPck_.ChkSum;
            outPck_.Bytes[outPck_.Size++] = STOPFLAG;
            if (outPck_.Size < 4)
                return false;                           // overflow occured
            outPackage = outPck_;
            return true;
        }

        public bool GetPck(ref serialPck inPackage)
        {
            inPackage = inPck_;
            return true;
        }

        public int ByteImport(byte newByte)
        {
            //--- first check if a "sized" data package is processed ----------------------------
            if (!((rxPck_.Flags & PCKCMPL) != 0)               // it's a new package
                    && !((rxPck_.ID & IDMESSAGE) != 0)         //   it's data package
                    && ((rxPck_.ID & IDSIZE) != 0)              //   it's a package with size info
                    && (rxPck_.SizeCnt < rxPck_.Size))   //   it's not the the stop flag
            {
                if ((rxPck_.Flags & PCKSIZE) != 0)             // the size is already received
                {
                    rxPck_.Bytes[rxPck_.SizeCnt++] = newByte;
                }
                else                                    // the actual byte is the size
                {
                    if (newByte != 0)
                    {
                        rxPck_.Size = (ushort)(newByte - 4);      // (data +START +ID +SIZE +STOP)
                    }
                    else
                    {
                        rxPck_.Size = extPckSize;       // SetPckSize() !!!
                    }
                    rxPck_.Flags += PCKSIZE;            // TODO what does size specify exactly ???
                    return PCKSIZE;
                }
                return 0;
            }
            else if (!((rxPck_.Flags & PCKCMPL) != 0)               // it's a new package
                    && !((rxPck_.ID & IDMESSAGE) != 0)         //   it's data package
                    && ((rxPck_.ID & IDSIZE) != 0)              //   it's a package with size info
                    && (rxPck_.SizeCnt >= rxPck_.Size))
            {
                if (newByte == STOPFLAG)
                {
                    inPck_ = rxPck_;
                    rxPck_ = new serialPck();
                    return PCKCMPL;
                }
            }
            //--- normal (no sized) package ------------------------------------------------
            switch (newByte)
            {
                case STARTFLAG:                     // always do (STARTFLAG)
                    rxPck_.ID = 0;                  //  clear IDenntity Byte
                    rxPck_.Size = 1;    //TODO why ???
                    rxPck_.SizeCnt = 0;             //  reset DataBuffer
                    rxPck_.ChkSum = 0;
                    if ((rxPck_.Flags & PCKCMPL) != 0)     // prev. Pack. complete ?
                    {
                        rxPck_.Flags = 0;           //  reset Command Flags
                    }
                    else
                    {                               // no -> StopError
                        rxPck_.Flags = 0;           //  reset Command Flags
                        return PERRSTOP;
                    }
                    break;
                case ESCFLAG:
                    if (((rxPck_.Flags & PCKCMPL) != 0) || ((rxPck_.Flags & PCKESC) != 0))
                    {
                        rxPck_.Flags |= PERRCMD;    // error
                        return PERRCMD;
                    }
                    else
                        rxPck_.Flags |= PCKESC;     // set EscapeFlag
                    break;
                case STOPFLAG:
                    if (((rxPck_.Flags & PCKCMPL) != 0) || (rxPck_.SizeCnt < 1))
                    {
                        rxPck_.Flags |= PERRSTART;  // missing StartFlag
                        return PERRSTART;
                    }
                    else
                    {
                        rxPck_.Flags |= PCKCMPL;    // set StopFlag
                        if ((rxPck_.ID & IDSIZE) != 0)
                        {
                            inPck_ = rxPck_;
                            return PCKCMPL;
                        }
                        rxPck_.ChkSum = rxPck_.ID;  //  and calc. CHKSUM
                        for (int j = 0; j < (rxPck_.SizeCnt - 1); j++)
                            if ((rxPck_.Bytes[j] == STARTFLAG)
                                    || (rxPck_.Bytes[j] == STOPFLAG)
                                    || (rxPck_.Bytes[j] == ESCFLAG)
                                    )
                                rxPck_.ChkSum += (byte)(rxPck_.Bytes[j] & ~ESCMASK);
                            else
                                rxPck_.ChkSum += rxPck_.Bytes[j];
                        if (rxPck_.ChkSum != rxPck_.Bytes[--rxPck_.SizeCnt])
                        {                               // remove CHKSUM
                            rxPck_.Flags |= PERRCHKSUM; // CheckSumError
                            //return PERRCHKSUM;
                            return PCKCMPL;
                        }
                        else
                        {
                            rxPck_.Size = rxPck_.SizeCnt;
                            inPck_ = rxPck_;
                            return PCKCMPL;
                        }
                    }
                default:
                    if ((rxPck_.Flags & PCKCMPL) != 0)
                    {                                   // StartError
                        rxPck_.Flags |= PERRSTART;      //   (missing flag)
                        return PERRSTART;
                    }
                    if (!(rxPck_.Flags != 0))                  // new transmission
                    {                                   //   -> this is PackageID
                        rxPck_.Flags = PCKID;           //   set flag
                        rxPck_.ID = newByte;
                        return PCKID;
                    }
                    else                                // running transmisssion
                    {
                        if (!((rxPck_.ID & IDSIZE) != 0)) //its not a package with size info
                        {
                            rxPck_.Bytes[(++rxPck_.SizeCnt) - 1] = newByte; /* TODO : ??? ++C -1 ??? */
                            if ((rxPck_.Flags & PCKESC) != 0)      //   check EscapeFlag
                            {
                                rxPck_.Bytes[rxPck_.SizeCnt - 1] |= ESCMASK;
                                rxPck_.Flags = (byte)(~PCKESC & rxPck_.Flags);    //   clear EscapeFlag
                            }
                        }

                    }
                    break;// default
            }//switch (newByte)

            return 0;
        }

        public bool SetPckSize(ushort pckSize)
        {
            if (pckSize <= MAXPCKSIZE)
            {
                extPckSize = pckSize;
                return true;
            }
            else
            {
                return false;
            }
        }

        public char[] BuildMessage(byte Message)
        {
            int i = 0;
            char[] package = new char[5];
            serialPck message = new serialPck();
            message.ID += IDMESSAGE;
            message.Size = 1;
            message.Bytes[0] = Message;

            package[i++] = (char)STARTFLAG;
            package[i++] = (char)message.ID;
            package[i++] = (char)message.Bytes[0];
            package[i++] = (char)(message.ID + message.Bytes[0]); //CheckSum
            package[i++] = (char)STOPFLAG;


            return package;




        }
    }
}
