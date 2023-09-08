using Nautilus.Json;
using Nautilus.Json.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod
{
    internal class SaveData
    {
        Dictionary<string, (Base.Piece, float)> basePieces;
    }

    [FileName("SealSubSaveData")]
    internal class SaveCache : SaveDataCache
    {
        public Dictionary<string, SaveData> saves = new();
    }
}
