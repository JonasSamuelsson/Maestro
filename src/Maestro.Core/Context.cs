using System;
using System.Collections.Generic;
using System.Linq;
using Maestro.Utils;

namespace Maestro
{
   internal class Context : IContext, IDisposable
   {
      private readonly IContextContainer _contextContainer;
      private bool _disposed;

      public Context(int configVersion, long contextId, string name, IContextContainer contextContainer)
      {
         ConfigVersion = configVersion;
         ContextId = contextId;
         Name = name;
         _contextContainer = contextContainer;

         TypeStack = new TypeStack();
      }

      public int ConfigVersion { get; private set; }
      public long ContextId { get; private set; }
      public string Name { get; internal set; }
      public ITypeStack TypeStack { get; private set; }

      public bool CanGet(Type type)
      {
         if (_disposed) throw new ObjectDisposedException("context");
         return _contextContainer.CanGet(type, this);
      }

      public bool CanGet<T>()
      {
         return CanGet(typeof(T));
      }

      public object Get(Type type)
      {
         if (_disposed) throw new ObjectDisposedException("context");
         return _contextContainer.Get(type, this);
      }

      public T Get<T>()
      {
         return (T)Get(typeof(T));
      }

      public IEnumerable<object> GetAll(Type type)
      {
         if (_disposed) throw new ObjectDisposedException("context");
         return _contextContainer.GetAll(type, this);
      }

      public IEnumerable<T> GetAll<T>()
      {
         return GetAll(typeof(T)).Cast<T>().ToList();
      }

      public event Action Disposed;

      public void Dispose()
      {
         _disposed = true;
         var action = Disposed;
         if (action == null) return;
         action();
      }
   }
}