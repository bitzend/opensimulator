﻿/*
 * Copyright (c) 2008 Intel Corporation
 * All rights reserved.
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 *
 * -- Redistributions of source code must retain the above copyright
 *    notice, this list of conditions and the following disclaimer.
 * -- Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * -- Neither the name of the Intel Corporation nor the names of its
 *    contributors may be used to endorse or promote products derived from
 *    this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
 * ``AS IS'' AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
 * LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
 * PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO EVENT SHALL THE INTEL OR ITS
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
 * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
 * PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using OpenMetaverse;

namespace OpenSim.Grid.AssetInventoryServer
{
    public class InventoryBase
    {
    }

    public class InventoryFolder : InventoryBase
    {
        public string Name;
        public UUID Owner;
        public UUID ParentID;
        public UUID ID;
        public short Type;
        public ushort Version;

        [NonSerialized]
        public Dictionary<UUID, InventoryBase> Children = new Dictionary<UUID, InventoryBase>();

        public InventoryFolder()
        {
        }

        public InventoryFolder(string name, UUID ownerID, UUID parentID, short assetType)
        {
            ID = UUID.Random();
            Name = name;
            Owner = ownerID;
            ParentID = parentID;
            Type = assetType;
            Version = 1;
        }

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, ID);
        }
    }

    public class InventoryItem : InventoryBase
    {
        public UUID ID;
        public int InvType;
        public UUID Folder;
        public UUID Owner;
        public UUID Creator;
        public string Name;
        public string Description;
        public uint NextPermissions;
        public uint CurrentPermissions;
        public uint BasePermissions;
        public uint EveryOnePermissions;
        public uint GroupPermissions;
        public int AssetType;
        public UUID AssetID;
        public UUID GroupID;
        public bool GroupOwned;
        public int SalePrice;
        public byte SaleType;
        public uint Flags;
        public int CreationDate;

        public override string ToString()
        {
            return String.Format("{0} ({1})", Name, ID);
        }
    }

    public class InventoryCollection
    {
        public Dictionary<UUID, InventoryFolder> Folders;
        public Dictionary<UUID, InventoryItem> Items;
        public UUID UserID;
    }
}
