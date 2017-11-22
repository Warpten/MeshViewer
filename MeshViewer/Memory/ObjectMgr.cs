using MeshViewer.Memory.Entities;
using MeshViewer.Memory.Enums;
using MeshViewer.Memory.Offsets;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MeshViewer.Memory
{
    public sealed class ObjectMgr
    {
        private const int NextObject    = 0x3C;
        private const int FirstObject   = 0xC;
        private const int LocalGUID     = 0xC8;

        private IntPtr _currentManager;

        private ulong _localGUID;

        private Dictionary<ulong, CGObject_C> _entities = new Dictionary<ulong, CGObject_C>();

        /// <summary>
        /// Called when an entity's base address pointer is refreshed.
        /// </summary>
        public event Action<CGObject_C> OnEntityUpdated;

        /// <summary>
        /// Called when a given entity despawns. Not that at that point,
        /// any updatefield is garbage since it hasn't been cached.
        /// Caching updatefields would be too much of a performance hit.
        /// </summary>
        public event Action<CGObject_C> OnEntityDespawn;

        /// <summary>
        /// Called when a given entity spawns (or enters visiblity range)
        /// </summary>
        public event Action<CGObject_C> OnEntitySpawn;

        /// <summary>
        /// This event triggers every time the object manager gets polled
        /// for new/removed entities.
        /// </summary>
        public event Action OnWorldUpdate;

        public static int UpdateFrequency { get; set; } = 15;

        public ObjectMgr()
        {
            if (!InGame)
                return;
        }

        ~ObjectMgr()
        {
            _entities.Clear();
        }

        /// <summary>
        /// Returns the entity that matches the provided GUID.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="objectGUID">The GUID of the expected entity.</param>
        /// <returns>An instance of <see cref="CGObject_C"/> or its children.</returns>
        public T GetEntity<T>(ulong objectGUID) where T : CGObject_C
        {
            _entities.TryGetValue(objectGUID, out CGObject_C instance);
            return (T)instance;
        }

        /// <summary>
        /// Returns the entity that matches the provided GUID.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="objectGUID">The GUID of the expected entity.</param>
        /// <returns>An instance of <see cref="CGObject_C"/> or its children.</returns>
        public T GetEntity<T>(ObjectGuid objectGUID) where T : CGObject_C => GetEntity<T>(objectGUID.Value);

        /// <summary>
        /// A direct pointer to the current player.
        /// </summary>
        public CGPlayer_C LocalPlayer => GetEntity<CGPlayer_C>(_localGUID);

        /// <summary>
        /// An enumeration of all known units.
        /// </summary>
        public IEnumerable<CGUnit_C> Units => _entities.Values.Where(obj => obj.Type == ObjectType.Unit).Select(o => o.ToUnit());

        /// <summary>
        /// An enumeration of all known players.
        /// </summary>
        public IEnumerable<CGPlayer_C> Players => _entities.Values.Where(obj => obj.Type == ObjectType.Player).Select(o => o.ToPlayer());

        /// <summary>
        /// An enumeration of all known items.
        /// </summary>
        public IEnumerable<CGItem_C> Items => _entities.Values.Where(obj => obj.Type == ObjectType.Item).Select(o => o.ToItem());

        /// <summary>
        /// An enumeration of all known containers.
        /// </summary>
        public IEnumerable<CGContainer_C> Containers => _entities.Values.Where(obj => obj.Type == ObjectType.Container).Select(o => o.ToContainer());

        /// <summary>
        /// An enumeration of all known game objects.
        /// </summary>
        public IEnumerable<CGGameObject_C> GameObjects => _entities.Values.Where(obj => obj.Type == ObjectType.GameObject).Select(o => o.ToGameObject());

        /// <summary>
        /// An enumeration of all known entities.
        /// </summary>
        public IEnumerable<CGObject_C> Entities => _entities.Values;

        /// <summary>
        /// Enumerates over every entities.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<IntPtr> Enumerate()
        {
            var currentObject = Game.Read<IntPtr>(_currentManager + FirstObject, true);

            while (currentObject.ToInt32() != uint.MinValue && currentObject.ToInt32() % 2 == uint.MinValue)
            {
                yield return currentObject;

                currentObject = Game.Read<IntPtr>(currentObject.ToInt32() + NextObject, true);
            }
        }

        /// <summary>
        /// Called every few milliseconds. This function updates the local cache of known entities.
        /// </summary>
        public void Update()
        {
            _currentManager = Game.Read<IntPtr>(Game.Read<int>(Cataclysm.CurMgrPointer) + Cataclysm.CurMgrOffset, true);
            _localGUID = Game.Read<ulong>(_currentManager + LocalGUID, true);

            foreach (var oldEntitiy in _entities)
                oldEntitiy.Value.BaseAddressUpdated = false;

            foreach (var newEntityPtr in Enumerate())
            {
                var updateFieldOffset = Game.Read<IntPtr>(newEntityPtr + 0xC, true);
                var objectGuid = Game.Read<ObjectGuid>(updateFieldOffset, true);

                if (_entities.ContainsKey(objectGuid))
                {
                    _entities[objectGuid].UpdateBaseAddress(newEntityPtr);
                    OnEntityUpdated?.Invoke(_entities[objectGuid]);
                }
                else
                {
                    switch (Game.Read<ObjectType>(newEntityPtr + 0x14, true))
                    {
                        default:
                        case ObjectType.Object:
                            _entities[objectGuid] = new CGObject_C(newEntityPtr);
                            break;
                        case ObjectType.Unit:
                            _entities[objectGuid] = new CGUnit_C(newEntityPtr);
                            break;
                        case ObjectType.Player:
                            _entities[objectGuid] = new CGPlayer_C(newEntityPtr);
                            break;
                        case ObjectType.GameObject:
                            _entities[objectGuid] = new CGGameObject_C(newEntityPtr);
                            break;
                        case ObjectType.Container:
                            _entities[objectGuid] = new CGContainer_C(newEntityPtr);
                            break;
                        case ObjectType.Item:
                            _entities[objectGuid] = new CGItem_C(newEntityPtr);
                            break;
                    }

                    OnEntitySpawn?.Invoke(_entities[objectGuid]);
                    _entities[objectGuid].OnSpawn();
                }
            }

            foreach (var removalKey in _entities.Where(kv => !kv.Value.BaseAddressUpdated).Select(kv => kv.Key).ToList())
            {
                OnEntityDespawn?.Invoke(_entities[removalKey]);
                _entities[removalKey].OnDespawn();

                _entities.Remove(removalKey);
            }

            OnWorldUpdate?.Invoke();
        }

        public bool InGame => Game.Read<byte>(Cataclysm.OnLoginScreen) == 0;
        public int CurrentMap => Game.Read<int>(_currentManager + Cataclysm.CurMapoffset, true);
    }
}
