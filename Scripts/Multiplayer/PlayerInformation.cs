using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace godotmultiplayerplatformer.Scripts.Multiplayer
{
    [Serializable]
    internal class PlayerInformation
    {
        public long Id { get; set; }
        public string Name { get; set; }

        public PlayerInformation(long id, string name)
        {
            Id = id;
            Name = name;
        }

        public PlayerInformation(long id)
        {
            Id = id;
            Name = $"UnknownPlayerInformation{id}";
        }

        public PlayerInformation() { }
    }
}
