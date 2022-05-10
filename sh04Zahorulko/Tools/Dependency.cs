using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace sh04Zahorulko
{
    public delegate bool CanAssign<T>(in T oldValue, in T newValue);
    public class DependencyCell<T>
    {
        private T value;
        public T Value
        {
            get => value;
            set
            {
                try
                {
                    semaphore.Wait();
                    if ((canAssign?.Invoke(in this.value, in value) ?? true))
                    {
                        this.value = value;
                        foreach (var r in receivers)
                        {
                            if (r.TryGetTarget(out var rVal))
                            {
                                rVal.Invoke(this.value);
                            }
                        }
                    }
                }
                finally
                {
                    semaphore.Release(1);
                }
            }
        }
        private readonly SemaphoreSlim semaphore = new(1);

        private readonly CanAssign<T>? canAssign;

        private readonly List<WeakReference<Dependency<T>>> receivers = new();
        public DependencyCell<T> Add(Dependency<T> receiver)
        {
            try
            {
                semaphore.Wait();
                if (!receivers.Any(v => v.TryGetTarget(out var rec) && rec == receiver))
                    receivers.Add(new(receiver));
                return this;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public DependencyCell<T> Remove(Dependency<T> receiver)
        {
            try
            {
                semaphore.Wait();
                int index = -1;
                for (int i = 0; i < receivers.Count; ++i)
                {
                    if (receivers[i].TryGetTarget(out var v) && v == receiver)
                        index = i;
                }
                if (index != -1)
                    receivers.RemoveAt(index);
                return this;
            }
            finally
            {
                semaphore.Release(1);
            }
        }

        internal DependencyCell(T value, CanAssign<T>? canAssign = null) => (this.value, this.canAssign) = (value, canAssign);
    }
    public class Dependency<T>
    {
        private DependencyCell<T> cell;
        public T Value
        {
            get => cell.Value;
            set => cell.Value = value;
        }
        private (string name, Action<string> notification)? notify;
        private readonly Action<T>? postAction;

        internal void Invoke(T value)
        {
            notify?.notification.Invoke(notify.Value.name);
            var type = typeof(T);
            postAction?.Invoke(value);
        }

        private Dependency(DependencyCell<T> cell, (string name, Action<string> notification)? notify = null, Action<T>? postAction = null)
        {
            this.cell = cell;
            this.notify = notify;
            this.postAction = postAction;
            cell.Add(this);
            Invoke(cell.Value);
        }

        public Dependency(T value, (string name, Action<string> notification)? notify = null, Action<T>? postAction = null, CanAssign<T>? canAssign = null)
            : this(new DependencyCell<T>(value, canAssign), notify, postAction)
        { }

        ~Dependency()
        {
            cell.Remove(this);
        }

        public static implicit operator T(Dependency<T> d) => d.Value;

        public Dependency<T> Copy((string name, Action<string> notification)? notify = null, Action<T>? postAction = null)
            => new(cell, notify, postAction);
    }
}
