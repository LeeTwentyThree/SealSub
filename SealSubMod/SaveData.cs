using Nautilus.Json;
using Nautilus.Json.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SealSubMod
{
    [Serializable]
    public class SaveData
    {
        public Dictionary<string, BasePieceSaveData> basePieces = new();

        //first key is console gameobject name, second key is slot
        public Dictionary<string, Dictionary<string, TechType>> modules = new();

        [Serializable]
        public class BasePieceSaveData
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
        public SaveCache()
        {
            OnFinishedLoading += (object _, JsonFileEventArgs _) => saves.ForEach((entry) => Plugin.Logger.LogMessage($"key {entry.Key}, value {entry.Value}, base pieces {entry.Value?.basePieces}, count {entry.Value?.basePieces?.Count}"));
        }
        public Dictionary<string, SaveData> saves = new();
    }
}
