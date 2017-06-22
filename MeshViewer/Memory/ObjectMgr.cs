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

        public event Action<CGObject_C> OnUpdate;
        public event Action<CGObject_C> OnDespawn;
        public event Action<CGObject_C> OnSpawn;
        public event Action OnUpdateTick;

        public static int UpdateFrequency { get; set; } = 50;

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
        private IEnumerable<CGObject_C> Enumerate()
        {
            var currentObject = (uint)Game.Read<IntPtr>(_currentManager + FirstObject, true);

            _localGUID = Game.Read<ulong>(_currentManager + LocalGUID, true);

            while (currentObject != uint.MinValue && currentObject % 2 == uint.MinValue)
            {
                CGObject_C entityObject;
                switch (Game.Read<ObjectType>((int)(currentObject + 0x14), true))
                {
                    default:
                    case ObjectType.Object:
                        entityObject = new CGObject_C(new IntPtr(currentObject));
                        break;
                    case ObjectType.Unit:
                        entityObject = new CGUnit_C(new IntPtr(currentObject));
                        break;
                    case ObjectType.Player:
                        entityObject = new CGPlayer_C(new IntPtr(currentObject));
                        break;
                    case ObjectType.GameObject:
                        entityObject = new CGGameObject_C(new IntPtr(currentObject));
                        break;
                    case ObjectType.Container:
                        entityObject = new CGContainer_C(new IntPtr(currentObject));
                        break;
                    case ObjectType.Item:
                        entityObject = new CGItem_C(new IntPtr(currentObject));
                        break;
                }

                yield return entityObject;

                currentObject = (uint)Game.Read<IntPtr>((int)(currentObject + NextObject), true);
            }
        }

        /// <summary>
        /// Called every few milliseconds. This function updates the local cache of known entities.
        /// </summary>
        public void Update()
        {
            _currentManager = Game.Read<IntPtr>(Game.Read<int>(Cataclysm.CurMgrPointer) + Cataclysm.CurMgrOffset, true);
            _localGUID = Game.Read<ulong>(_currentManager + LocalGUID, true);

            var newEntities = Enumerate().ToDictionary(@object => @object.OBJECT_FIELD_GUID.Value);

            foreach (var oldEntity in _entities)
            {
                if (newEntities.ContainsKey(oldEntity.Key))
                {
                    oldEntity.Value.UpdateBaseAddress(newEntities[oldEntity.Key].BaseAddress);

                    OnUpdate?.Invoke(oldEntity.Value);
                }
                else
                    OnDespawn?.Invoke(oldEntity.Value);
            }

            foreach (var newEntity in newEntities)
                if (!_entities.ContainsKey(newEntity.Key))
                    OnSpawn?.Invoke(newEntity.Value);

            _entities.Clear();
            _entities = newEntities;

            OnUpdateTick?.Invoke();
        }

        public bool InGame => Game.Read<byte>(Cataclysm.OnLoginScreen) == 0;
        public int CurrentMap => Game.Read<int>(_currentManager + Cataclysm.CurMapoffset, true);
    }
}
