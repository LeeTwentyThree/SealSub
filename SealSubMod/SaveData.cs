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
        public Dictionary<string, BasePieceSaveData> basePieces = new();

        internal class BasePieceSaveData
        {
            public BasePieceSaveData(Base.Piece pieceType = Base.Piece.Invalid, float constructedAmount = 0, Base.Direction direction = Base.Direction.North)
            {
                this.pieceType = pieceType;
                this.constructedAmount = constructedAmount;
                this.direction = direction;
            }
            public Base.Piece pieceType;
            public float constructedAmount;
            public Base.Direction direction;
        }
    }

    [FileName("SealSubSaveData")]
    internal class SaveCache : SaveDataCache
    {
        public Dictionary<string, SaveData> saves = new();
    }
}
